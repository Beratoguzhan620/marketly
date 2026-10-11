using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<UserProfileDto>> GetProfileAsync(int userId);

        Task<ServiceResult> UpdateProfileAsync(int userId, UserUpdateRequest request);

        Task<ServiceResult<List<UserListDto>>> GetUsersAsync();

        Task<ServiceResult> SetUserEnabledAsync(int userId, bool enabled);

        Task<ServiceResult> RequestSellerRoleAsync(int userId);

        Task<ServiceResult> ApproveSellerRequestAsync(int userId);
    }
}
