using App.Application.Models.Dtos;
using App.Application.Pricing;
using App.Application.Models.Requests;
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
        DataRepository<ProductEntity> productRepo) : IOrderService
    {
        public async Task<ServiceResult<OrderDto>> CreateOrderAsync(int userId, OrderCreateRequest request)
        {
            var cartItems = await cartItemRepo.GetAll()
                .Include(ci => ci.Product.Discount)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var unitPrices = cartItems.ToDictionary(
                ci => ci.Id,
                ci => ProductPricing.Apply(ci.Product.Price, ProductPricing.ActiveRate(ci.Product.Discount, now)));

            if (cartItems.Count == 0)
            {
                return ServiceResult<OrderDto>.ValidationError("Sepetinizde ürün bulunmuyor.");
            }

            foreach (var cartItem in cartItems)
            {
                if (!cartItem.Product.Enabled || cartItem.Product.StockAmount < cartItem.Quantity)
                {
                    return ServiceResult<OrderDto>.ValidationError($"'{cartItem.Product.Name}' ürünü için yeterli stok bulunmuyor.");
                }
            }

            // Sipariş, sipariş ürünleri, stok güncelleme ve sepet temizleme işlemlerinin
            // birlikte başarılı olması için transaction içerisinde gerçekleştirilir.
            await using var transaction = await orderRepo.BeginTransactionAsync();

            try
            {
                var order = new OrderEntity
                {
                    UserId = userId,
                    Address = request.Address,
                    OrderCode = await CreateUniqueOrderCodeAsync(),
                };

                order = await orderRepo.AddAsync(order);

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
                    TotalPrice = cartItems.Sum(ci => ci.Quantity * unitPrices[ci.Id]),
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
                .Select(o => new OrderDto
                {
                    OrderCode = o.OrderCode,
                    Address = o.Address,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                    TotalProducts = o.OrderItems.Count,
                    TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity),
                })
                .OrderByDescending(o => o.CreatedAt)
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

        private async Task<string> CreateUniqueOrderCodeAsync()
        {
            var orderCode = Guid.NewGuid().ToString("n")[..16].ToUpperInvariant();

            while (await orderRepo.GetAll().AnyAsync(o => o.OrderCode == orderCode))
            {
                orderCode = Guid.NewGuid().ToString("n")[..16].ToUpperInvariant();
            }

            return orderCode;
        }
    }
}
