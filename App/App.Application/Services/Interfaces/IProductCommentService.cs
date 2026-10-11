using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IProductCommentService
    {
        Task<List<ProductCommentDto>> GetApprovedCommentsAsync(int productId);

        Task<List<ProductCommentDto>> GetPendingCommentsAsync();

        Task<ServiceResult> AddCommentAsync(int userId, int productId, ProductCommentCreateRequest request);

        Task<ServiceResult> ApproveCommentAsync(int commentId);
    }
}
