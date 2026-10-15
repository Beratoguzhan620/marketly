using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Pricing;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class OrderService(
        DataRepository<CartItemEntity> cartItemRepo,
        DataRepository<OrderEntity> orderRepo,
        DataRepository<OrderItemEntity> orderItemRepo,
        DataRepository<ProductEntity> productRepo,
        DataRepository<CouponEntity> couponRepo,
        TimeProvider timeProvider) : IOrderService
    {
        public async Task<ServiceResult<OrderDto>> CreateOrderAsync(int userId, OrderCreateRequest request)
        {
            var cartItems = await cartItemRepo.GetAll()
                .Include(ci => ci.Product.Discount)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return ServiceResult<OrderDto>.ValidationError("Sepetinizde ürün bulunmuyor.");
            }

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var unitPrices = cartItems.ToDictionary(
                ci => ci.Id,
                ci => ProductPricing.Apply(ci.Product.Price, ProductPricing.ActiveRate(ci.Product.Discount, now)));

            foreach (var cartItem in cartItems)
            {
                if (!cartItem.Product.Enabled || cartItem.Product.StockAmount < cartItem.Quantity)
                {
                    return ServiceResult<OrderDto>.ValidationError($"'{cartItem.Product.Name}' ürünü için yeterli stok bulunmuyor.");
                }
            }

            var subtotal = cartItems.Sum(ci => ci.Quantity * unitPrices[ci.Id]);

            CouponEntity? coupon = null;
            var couponCode = CouponPricing.Normalize(request.CouponCode);
            if (couponCode.Length > 0)
            {
                coupon = await couponRepo.GetAll().AsNoTracking().FirstOrDefaultAsync(c => c.Code == couponCode);
                var couponError = CouponPricing.Check(coupon, subtotal, now);
                if (couponError is not null)
                {
                    return ServiceResult<OrderDto>.ValidationError(couponError);
                }
            }

            var discountAmount = coupon is null ? 0m : CouponPricing.Discount(subtotal, coupon.DiscountRate);

            // The order, its items, the stock change, the coupon usage and the emptied cart
            // must all succeed together, so they share one transaction.
            await using var transaction = await orderRepo.BeginTransactionAsync();

            try
            {
                if (coupon is not null)
                {
                    // Claim one use atomically so two parallel checkouts cannot exceed the limit.
                    var claimed = await couponRepo.GetAll()
                        .Where(c => c.Id == coupon.Id && (c.UsageLimit == null || c.UsedCount < c.UsageLimit))
                        .ExecuteUpdateAsync(s => s.SetProperty(c => c.UsedCount, c => c.UsedCount + 1));

                    if (claimed == 0)
                    {
                        await transaction.RollbackAsync();
                        return ServiceResult<OrderDto>.ValidationError("Bu indirim kodunun kullanım hakkı dolmuş.");
                    }
                }

                var order = await orderRepo.AddAsync(new OrderEntity
                {
                    UserId = userId,
                    Address = request.Address,
                    // Temporary unique value; replaced below once the id is known.
                    OrderCode = "TMP-" + Guid.NewGuid().ToString("n"),
                    Status = OrderStatus.Preparing,
                    CouponCode = coupon?.Code,
                    DiscountAmount = discountAmount,
                });

                order.OrderCode = FormatOrderCode(order.CreatedAt, order.Id);
                await orderRepo.UpdateAsync(order);

                foreach (var cartItem in cartItems)
                {
                    await orderItemRepo.AddAsync(new OrderItemEntity
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = unitPrices[cartItem.Id],
                    });

                    cartItem.Product.StockAmount -= cartItem.Quantity;
                    await productRepo.UpdateAsync(cartItem.Product);

                    await cartItemRepo.DeleteAsync(cartItem.Id);
                }

                await transaction.CommitAsync();

                return ServiceResult<OrderDto>.Ok(new OrderDto
                {
                    OrderCode = order.OrderCode,
                    Address = order.Address,
                    CreatedAt = order.CreatedAt,
                    Status = order.Status,
                    Subtotal = subtotal,
                    DiscountAmount = discountAmount,
                    TotalProducts = cartItems.Count,
                    TotalQuantity = cartItems.Sum(ci => ci.Quantity),
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                return ServiceResult<OrderDto>.Fail("Sipariş oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
        {
            return await orderRepo.GetAll()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    OrderCode = o.OrderCode,
                    Address = o.Address,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    Subtotal = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                    DiscountAmount = o.DiscountAmount,
                    TotalProducts = o.OrderItems.Count,
                    TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity),
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<OrderDetailDto>> GetOrderDetailAsync(int userId, string orderCode)
        {
            var order = await orderRepo.GetAll()
                .Where(o => o.UserId == userId && o.OrderCode == orderCode)
                .Select(o => new OrderDetailDto
                {
                    OrderCode = o.OrderCode,
                    CreatedAt = o.CreatedAt,
                    Address = o.Address,
                    Status = o.Status,
                    StatusUpdatedAt = o.StatusUpdatedAt,
                    CouponCode = o.CouponCode,
                    DiscountAmount = o.DiscountAmount,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return order is null
                ? ServiceResult<OrderDetailDto>.NotFound()
                : ServiceResult<OrderDetailDto>.Ok(order);
        }

        /// <summary>Human-friendly, sortable order number such as MK-2026-0007.</summary>
        public static string FormatOrderCode(DateTime createdAt, int id) =>
            string.Create(System.Globalization.CultureInfo.InvariantCulture, $"MK-{createdAt:yyyy}-{id:D4}");
    }
}
