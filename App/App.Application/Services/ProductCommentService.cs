using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class ProductCommentService(
        DataRepository<ProductCommentEntity> commentRepo,
        DataRepository<ProductEntity> productRepo,
        DataRepository<OrderItemEntity> orderItemRepo) : IProductCommentService
    {
        public async Task<List<ProductCommentDto>> GetApprovedCommentsAsync(int productId)
        {
            return await commentRepo.GetAll()
                .Include(c => c.User)
                .Where(c => c.ProductId == productId && c.IsConfirmed)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ProductCommentDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    UserName = c.User.FirstName + " " + c.User.LastName,
                    IsConfirmed = c.IsConfirmed,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<ProductCommentDto>> GetPendingCommentsAsync()
        {
            return await commentRepo.GetAll()
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => !c.IsConfirmed)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new ProductCommentDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    UserName = c.User.FirstName + " " + c.User.LastName,
                    IsConfirmed = c.IsConfirmed,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ServiceResult> AddCommentAsync(int userId, int productId, ProductCommentCreateRequest request)
        {
            if (request.StarCount is < 1 or > 5)
            {
                return ServiceResult.ValidationError("Yıldız değeri 1 ile 5 arasında olmalıdır.");
            }

            var productExists = await productRepo.GetAll().AnyAsync(p => p.Id == productId);

            if (!productExists)
            {
                return ServiceResult.NotFound("Ürün bulunamadı.");
            }

            var hasPurchased = await orderItemRepo.GetAll()
                .AnyAsync(oi => oi.ProductId == productId && oi.Order.UserId == userId);

            if (!hasPurchased)
            {
                return ServiceResult.Forbidden("Yalnızca satın aldığınız ürünlere yorum yapabilirsiniz.");
            }

            var alreadyCommented = await commentRepo.GetAll()
                .AnyAsync(c => c.ProductId == productId && c.UserId == userId);

            if (alreadyCommented)
            {
                return ServiceResult.ValidationError("Bu ürüne zaten yorum yaptınız.");
            }

            var comment = new ProductCommentEntity
            {
                ProductId = productId,
                UserId = userId,
                Text = request.Text,
                StarCount = request.StarCount,
                IsConfirmed = false,
            };

            await commentRepo.AddAsync(comment);

            return ServiceResult.Ok("Yorumunuz alındı. Onaylandıktan sonra yayınlanacaktır.");
        }

        public async Task<ServiceResult> ApproveCommentAsync(int commentId)
        {
            var comment = await commentRepo.GetByIdAsync(commentId);

            if (comment is null)
            {
                return ServiceResult.NotFound("Yorum bulunamadı.");
            }

            comment.IsConfirmed = true;
            await commentRepo.UpdateAsync(comment);

            return ServiceResult.Ok("Yorum onaylandı.");
        }
    }
}
