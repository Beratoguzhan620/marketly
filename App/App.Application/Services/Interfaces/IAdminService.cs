using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    /// <summary>Read models and bulk actions for the admin panel.</summary>
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboardAsync();

        Task<PagedResult<AdminProductRowDto>> GetProductsAsync(AdminProductQuery query);

        Task<ServiceResult> SetProductsEnabledAsync(IReadOnlyCollection<int> productIds, bool enabled);

        Task<PagedResult<AdminUserRowDto>> GetUsersAsync(AdminUserQuery query);

        Task<ServiceResult> RejectSellerRequestAsync(int userId);

        Task<PagedResult<AdminCommentRowDto>> GetCommentsAsync(AdminCommentQuery query);

        Task<ServiceResult> ApproveCommentsAsync(IReadOnlyCollection<int> commentIds);

        Task<ServiceResult> DeleteCommentsAsync(IReadOnlyCollection<int> commentIds);
    }
}
