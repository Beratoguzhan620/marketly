using App.Application.Models.Dtos;
using App.Application.Orders;
using App.Application.Pricing;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class AdminService(
        DataRepository<OrderEntity> orderRepo,
        DataRepository<OrderItemEntity> orderItemRepo,
        DataRepository<UserEntity> userRepo,
        DataRepository<ProductEntity> productRepo,
        DataRepository<ProductCommentEntity> commentRepo,
        DataRepository<BlogCommentEntity> blogCommentRepo,
        DataRepository<CouponEntity> couponRepo,
        TimeProvider timeProvider) : IAdminService
    {
        private const int MaxBulkSize = 100;
        private const int ChartMonths = 12;

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var firstMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(ChartMonths - 1));

            // Cancelled orders do not count as sales; coupon discounts are taken off the total.
            var countedOrders = orderRepo.GetAll().Where(o => o.Status != OrderStatus.Cancelled);

            var recentSales = await countedOrders
                .Where(o => o.CreatedAt >= firstMonth)
                .Select(o => new
                {
                    o.CreatedAt.Year,
                    o.CreatedAt.Month,
                    Total = o.OrderItems.Sum(i => i.UnitPrice * i.Quantity) - o.DiscountAmount,
                })
                .ToListAsync();

            var sales = recentSales
                .GroupBy(o => new { o.Year, o.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Orders = g.Count(), Revenue = g.Sum(o => o.Total) })
                .ToList();

            var monthly = Enumerable.Range(0, ChartMonths)
                .Select(offset => firstMonth.AddMonths(offset))
                .Select(month =>
                {
                    var row = sales.FirstOrDefault(s => s.Year == month.Year && s.Month == month.Month);
                    return new MonthlySalesDto
                    {
                        Year = month.Year,
                        Month = month.Month,
                        OrderCount = row?.Orders ?? 0,
                        Revenue = row?.Revenue ?? 0,
                    };
                })
                .ToList();

            var recentOrders = await orderRepo.GetAll()
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new AdminOrderSummaryDto
                {
                    OrderCode = o.OrderCode,
                    CustomerName = o.User.FirstName + " " + o.User.LastName,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Sum(i => i.Quantity),
                    Total = o.OrderItems.Sum(i => i.UnitPrice * i.Quantity) - o.DiscountAmount,
                    Status = o.Status,
                })
                .ToListAsync();

            return new AdminDashboardDto
            {
                OrderCount = await countedOrders.CountAsync(),
                Revenue = (await orderItemRepo.GetAll().Where(i => i.Order.Status != OrderStatus.Cancelled).SumAsync(i => (decimal?)(i.UnitPrice * i.Quantity)) ?? 0)
                    - (await countedOrders.SumAsync(o => (decimal?)o.DiscountAmount) ?? 0),
                UserCount = await userRepo.GetAll().CountAsync(),
                NewUsersLast30Days = await userRepo.GetAll().CountAsync(u => u.CreatedAt >= now.AddDays(-30)),
                ActiveProductCount = await productRepo.GetAll().CountAsync(p => p.Enabled),
                PendingCommentCount = await commentRepo.GetAll().CountAsync(c => !c.IsConfirmed),
                PendingSellerRequestCount = await userRepo.GetAll().CountAsync(u => u.HasSellerRequest),
                PendingBlogCommentCount = await blogCommentRepo.GetAll().CountAsync(c => !c.IsApproved),
                PreparingOrderCount = await orderRepo.GetAll().CountAsync(o => o.Status == OrderStatus.Preparing),
                MonthlySales = monthly,
                RecentOrders = recentOrders,
            };
        }

        public async Task<PagedResult<AdminProductRowDto>> GetProductsAsync(AdminProductQuery query)
        {
            var products = productRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                products = products.Where(p => p.Name.Contains(term) || (p.Seller.FirstName + " " + p.Seller.LastName).Contains(term));
            }

            if (query.CategoryId is not null)
                products = products.Where(p => p.CategoryId == query.CategoryId);

            products = query.Status switch
            {
                ActiveFilter.Active => products.Where(p => p.Enabled),
                ActiveFilter.Inactive => products.Where(p => !p.Enabled),
                _ => products,
            };

            return await PageAsync(
                products.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id).Select(p => new AdminProductRowDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    SellerName = p.Seller.FirstName + " " + p.Seller.LastName,
                    Price = p.Price,
                    Stock = p.StockAmount,
                    Enabled = p.Enabled,
                    ImageCount = p.Images.Count,
                    CreatedAt = p.CreatedAt,
                }),
                query);
        }

        public async Task<ServiceResult> SetProductsEnabledAsync(IReadOnlyCollection<int> productIds, bool enabled)
        {
            var check = CheckSelection(productIds);
            if (check is not null)
                return check;

            var updated = await productRepo.GetAll()
                .Where(p => productIds.Contains(p.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Enabled, enabled));

            return ServiceResult.Ok(enabled ? $"{updated} ürün satışa açıldı." : $"{updated} ürün satıştan kaldırıldı.");
        }

        public async Task<PagedResult<AdminUserRowDto>> GetUsersAsync(AdminUserQuery query)
        {
            var users = userRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                users = users.Where(u => (u.FirstName + " " + u.LastName).Contains(term) || u.Email.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(query.Role))
                users = users.Where(u => u.Role.Name == query.Role);

            if (query.OnlySellerRequests)
                users = users.Where(u => u.HasSellerRequest);

            users = query.Status switch
            {
                ActiveFilter.Active => users.Where(u => u.Enabled),
                ActiveFilter.Inactive => users.Where(u => !u.Enabled),
                _ => users,
            };

            return await PageAsync(
                users.OrderByDescending(u => u.HasSellerRequest).ThenBy(u => u.FirstName).ThenBy(u => u.LastName).Select(u => new AdminUserRowDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Role = u.Role.Name,
                    Enabled = u.Enabled,
                    HasSellerRequest = u.HasSellerRequest,
                    CreatedAt = u.CreatedAt,
                }),
                query);
        }

        public async Task<ServiceResult> RejectSellerRequestAsync(int userId)
        {
            var updated = await userRepo.GetAll()
                .Where(u => u.Id == userId && u.HasSellerRequest)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.HasSellerRequest, false));

            return updated == 0
                ? ServiceResult.NotFound("Bekleyen bir satıcı başvurusu bulunamadı.")
                : ServiceResult.Ok("Satıcı başvurusu reddedildi.");
        }

        public async Task<PagedResult<AdminCommentRowDto>> GetCommentsAsync(AdminCommentQuery query)
        {
            var comments = commentRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                comments = comments.Where(c => c.Text.Contains(term) || c.Product.Name.Contains(term));
            }

            comments = query.Status switch
            {
                CommentStatusFilter.Pending => comments.Where(c => !c.IsConfirmed),
                CommentStatusFilter.Approved => comments.Where(c => c.IsConfirmed),
                _ => comments,
            };

            return await PageAsync(
                comments.OrderByDescending(c => c.CreatedAt).Select(c => new AdminCommentRowDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    UserName = c.User.FirstName + " " + c.User.LastName,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    IsConfirmed = c.IsConfirmed,
                    CreatedAt = c.CreatedAt,
                }),
                query);
        }

        public async Task<ServiceResult> ApproveCommentsAsync(IReadOnlyCollection<int> commentIds)
        {
            var check = CheckSelection(commentIds);
            if (check is not null)
                return check;

            var updated = await commentRepo.GetAll()
                .Where(c => commentIds.Contains(c.Id) && !c.IsConfirmed)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsConfirmed, true));

            return ServiceResult.Ok($"{updated} yorum onaylandı.");
        }

        public async Task<ServiceResult> DeleteCommentsAsync(IReadOnlyCollection<int> commentIds)
        {
            var check = CheckSelection(commentIds);
            if (check is not null)
                return check;

            var deleted = await commentRepo.GetAll()
                .Where(c => commentIds.Contains(c.Id))
                .ExecuteDeleteAsync();

            return ServiceResult.Ok($"{deleted} yorum silindi.");
        }

        public async Task<PagedResult<AdminOrderRowDto>> GetOrdersAsync(AdminOrderQuery query)
        {
            var orders = orderRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                orders = orders.Where(o => o.OrderCode.Contains(term)
                    || (o.User.FirstName + " " + o.User.LastName).Contains(term)
                    || o.User.Email.Contains(term));
            }

            if (query.Status is OrderStatus status)
                orders = orders.Where(o => o.Status == status);

            return await PageAsync(
                orders.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id).Select(o => new AdminOrderRowDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CustomerName = o.User.FirstName + " " + o.User.LastName,
                    CustomerEmail = o.User.Email,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Sum(i => i.Quantity),
                    Subtotal = o.OrderItems.Sum(i => i.UnitPrice * i.Quantity),
                    DiscountAmount = o.DiscountAmount,
                    CouponCode = o.CouponCode,
                    Status = o.Status,
                    StatusUpdatedAt = o.StatusUpdatedAt,
                }),
                query);
        }

        public async Task<AdminOrderDetailDto?> GetOrderAsync(int orderId)
        {
            return await orderRepo.GetAll()
                .Where(o => o.Id == orderId)
                .Select(o => new AdminOrderDetailDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CustomerName = o.User.FirstName + " " + o.User.LastName,
                    CustomerEmail = o.User.Email,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Sum(i => i.Quantity),
                    Subtotal = o.OrderItems.Sum(i => i.UnitPrice * i.Quantity),
                    DiscountAmount = o.DiscountAmount,
                    CouponCode = o.CouponCode,
                    Status = o.Status,
                    StatusUpdatedAt = o.StatusUpdatedAt,
                    Address = o.Address,
                    Items = o.OrderItems.OrderBy(i => i.Id).Select(i => new OrderItemDto
                    {
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                    }).ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var current = await orderRepo.GetAll()
                .Where(o => o.Id == orderId)
                .Select(o => (OrderStatus?)o.Status)
                .FirstOrDefaultAsync();

            if (current is null)
                return ServiceResult.NotFound("Sipariş bulunamadı.");

            if (!OrderStatusText.CanMove(current.Value, status))
                return ServiceResult.ValidationError(
                    $"\"{OrderStatusText.Label(current.Value)}\" durumundaki sipariş \"{OrderStatusText.Label(status)}\" yapılamaz.");

            var now = timeProvider.GetUtcNow().UtcDateTime;
            await using var transaction = await orderRepo.BeginTransactionAsync();

            // The status check is repeated in the update so two admins cannot move the same order twice.
            var updated = await orderRepo.GetAll()
                .Where(o => o.Id == orderId && o.Status == current.Value)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(o => o.Status, status)
                    .SetProperty(o => o.StatusUpdatedAt, now));

            if (updated == 0)
                return ServiceResult.ValidationError("Sipariş bu arada başka biri tarafından güncellendi. Sayfayı yenileyin.");

            if (status == OrderStatus.Cancelled)
                await RestockAsync(orderId);

            await transaction.CommitAsync();

            return ServiceResult.Ok($"Sipariş durumu \"{OrderStatusText.Label(status)}\" olarak güncellendi.");
        }

        public async Task<PagedResult<AdminBlogCommentRowDto>> GetBlogCommentsAsync(AdminBlogCommentQuery query)
        {
            var comments = blogCommentRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                comments = comments.Where(c => c.Comment.Contains(term) || c.Name.Contains(term) || c.Blog.Title.Contains(term));
            }

            comments = query.Status switch
            {
                CommentStatusFilter.Pending => comments.Where(c => !c.IsApproved),
                CommentStatusFilter.Approved => comments.Where(c => c.IsApproved),
                _ => comments,
            };

            return await PageAsync(
                comments.OrderByDescending(c => c.CreatedAt).Select(c => new AdminBlogCommentRowDto
                {
                    Id = c.Id,
                    BlogId = c.BlogId,
                    BlogTitle = c.Blog.Title,
                    Name = c.Name,
                    Email = c.Email,
                    Comment = c.Comment,
                    IsApproved = c.IsApproved,
                    CreatedAt = c.CreatedAt,
                }),
                query);
        }

        public async Task<ServiceResult> ApproveBlogCommentsAsync(IReadOnlyCollection<int> commentIds)
        {
            var check = CheckSelection(commentIds);
            if (check is not null)
                return check;

            var updated = await blogCommentRepo.GetAll()
                .Where(c => commentIds.Contains(c.Id) && !c.IsApproved)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsApproved, true));

            return ServiceResult.Ok($"{updated} blog yorumu onaylandı.");
        }

        public async Task<ServiceResult> DeleteBlogCommentsAsync(IReadOnlyCollection<int> commentIds)
        {
            var check = CheckSelection(commentIds);
            if (check is not null)
                return check;

            var deleted = await blogCommentRepo.GetAll()
                .Where(c => commentIds.Contains(c.Id))
                .ExecuteDeleteAsync();

            return ServiceResult.Ok($"{deleted} blog yorumu silindi.");
        }

        public async Task<PagedResult<AdminCouponRowDto>> GetCouponsAsync(AdminCouponQuery query)
        {
            var coupons = couponRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                coupons = coupons.Where(c => c.Code.Contains(term));
            }

            coupons = query.Status switch
            {
                ActiveFilter.Active => coupons.Where(c => c.Enabled),
                ActiveFilter.Inactive => coupons.Where(c => !c.Enabled),
                _ => coupons,
            };

            return await PageAsync(
                coupons.OrderByDescending(c => c.Enabled).ThenByDescending(c => c.EndDate).Select(c => new AdminCouponRowDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    DiscountRate = c.DiscountRate,
                    MinimumOrderTotal = c.MinimumOrderTotal,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    UsageLimit = c.UsageLimit,
                    UsedCount = c.UsedCount,
                    Enabled = c.Enabled,
                }),
                query);
        }

        public async Task<ServiceResult> CreateCouponAsync(CouponCreateRequest request)
        {
            var code = CouponPricing.Normalize(request.Code);

            if (code.Length is < 4 or > CouponPricing.MaxCodeLength || !code.All(ch => ch is >= 'A' and <= 'Z' or >= '0' and <= '9'))
                return ServiceResult.ValidationError("Kod 4-32 karakter olmalı ve yalnızca harf (A-Z) ile rakam içermeli.");

            if (request.DiscountRate is < 1 or > 90)
                return ServiceResult.ValidationError("İndirim oranı %1 ile %90 arasında olmalı.");

            if (request.EndDate <= request.StartDate)
                return ServiceResult.ValidationError("Bitiş tarihi başlangıç tarihinden sonra olmalı.");

            if (request.MinimumOrderTotal is < 0)
                return ServiceResult.ValidationError("En az sepet tutarı negatif olamaz.");

            if (request.UsageLimit is < 1)
                return ServiceResult.ValidationError("Kullanım limiti en az 1 olmalı; sınırsız için boş bırakın.");

            if (await couponRepo.GetAll().AnyAsync(c => c.Code == code))
                return ServiceResult.ValidationError("Bu kod zaten kullanılıyor.");

            await couponRepo.AddAsync(new CouponEntity
            {
                Code = code,
                DiscountRate = request.DiscountRate,
                MinimumOrderTotal = request.MinimumOrderTotal,
                StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc),
                UsageLimit = request.UsageLimit,
                Enabled = true,
            });

            return ServiceResult.Ok($"{code} kodu oluşturuldu.");
        }

        public async Task<ServiceResult> SetCouponsEnabledAsync(IReadOnlyCollection<int> couponIds, bool enabled)
        {
            var check = CheckSelection(couponIds);
            if (check is not null)
                return check;

            var updated = await couponRepo.GetAll()
                .Where(c => couponIds.Contains(c.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.Enabled, enabled));

            return ServiceResult.Ok(enabled ? $"{updated} kod etkinleştirildi." : $"{updated} kod durduruldu.");
        }

        /// <summary>Puts the items of a cancelled order back into stock.</summary>
        private async Task RestockAsync(int orderId)
        {
            var items = await orderItemRepo.GetAll()
                .Where(i => i.OrderId == orderId)
                .Select(i => new { i.ProductId, i.Quantity })
                .ToListAsync();

            foreach (var item in items)
            {
                await productRepo.GetAll()
                    .Where(p => p.Id == item.ProductId)
                    .ExecuteUpdateAsync(s => s.SetProperty(
                        p => p.StockAmount,
                        p => p.StockAmount + item.Quantity > byte.MaxValue ? byte.MaxValue : (byte)(p.StockAmount + item.Quantity)));
            }
        }

        private static ServiceResult? CheckSelection(IReadOnlyCollection<int> ids)
        {
            if (ids.Count == 0)
                return ServiceResult.ValidationError("Önce en az bir kayıt seçin.");

            if (ids.Count > MaxBulkSize)
                return ServiceResult.ValidationError($"Tek seferde en fazla {MaxBulkSize} kayıt seçilebilir.");

            return null;
        }

        private static async Task<PagedResult<T>> PageAsync<T>(IQueryable<T> source, AdminPagedQuery query)
        {
            var pageSize = Math.Clamp(query.PageSize, 1, 100);
            var total = await source.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            var page = Math.Clamp(query.Page, 1, totalPages);

            return new PagedResult<T>
            {
                Items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
            };
        }
    }
}
