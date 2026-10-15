using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Data.Entities;

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

        Task<PagedResult<AdminOrderRowDto>> GetOrdersAsync(AdminOrderQuery query);

        Task<AdminOrderDetailDto?> GetOrderAsync(int orderId);

        Task<ServiceResult> UpdateOrderStatusAsync(int orderId, OrderStatus status);

        Task<PagedResult<AdminBlogCommentRowDto>> GetBlogCommentsAsync(AdminBlogCommentQuery query);

        Task<ServiceResult> ApproveBlogCommentsAsync(IReadOnlyCollection<int> commentIds);

        Task<ServiceResult> DeleteBlogCommentsAsync(IReadOnlyCollection<int> commentIds);

        Task<PagedResult<AdminCouponRowDto>> GetCouponsAsync(AdminCouponQuery query);

        Task<ServiceResult> CreateCouponAsync(CouponCreateRequest request);

        Task<ServiceResult> SetCouponsEnabledAsync(IReadOnlyCollection<int> couponIds, bool enabled);
    }
}
