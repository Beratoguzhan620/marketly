using App.Application.Models.Dtos;
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
        TimeProvider timeProvider) : IAdminService
    {
        private const int MaxBulkSize = 100;
        private const int ChartMonths = 12;

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var firstMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(ChartMonths - 1));

            var sales = await orderItemRepo.GetAll()
                .Where(i => i.Order.CreatedAt >= firstMonth)
                .GroupBy(i => new { i.Order.CreatedAt.Year, i.Order.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Orders = g.Select(i => i.OrderId).Distinct().Count(),
                    Revenue = g.Sum(i => i.UnitPrice * i.Quantity),
                })
                .ToListAsync();

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
                    Total = o.OrderItems.Sum(i => i.UnitPrice * i.Quantity),
                })
                .ToListAsync();

            return new AdminDashboardDto
            {
                OrderCount = await orderRepo.GetAll().CountAsync(),
                Revenue = await orderItemRepo.GetAll().SumAsync(i => (decimal?)(i.UnitPrice * i.Quantity)) ?? 0,
                UserCount = await userRepo.GetAll().CountAsync(),
                NewUsersLast30Days = await userRepo.GetAll().CountAsync(u => u.CreatedAt >= now.AddDays(-30)),
                ActiveProductCount = await productRepo.GetAll().CountAsync(p => p.Enabled),
                PendingCommentCount = await commentRepo.GetAll().CountAsync(c => !c.IsConfirmed),
                PendingSellerRequestCount = await userRepo.GetAll().CountAsync(u => u.HasSellerRequest),
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
