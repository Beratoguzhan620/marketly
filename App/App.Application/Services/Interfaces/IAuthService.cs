using App.Application.Models.Requests;
using App.Application.Models.Dtos;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult> RegisterAsync(RegisterRequest request);

        Task<ServiceResult<AuthUserDto>> ValidateLoginAsync(LoginRequest request);

        Task<ServiceResult> RequestPasswordResetAsync(string email);

        Task<ServiceResult<bool>> VerifyResetTokenAsync(string verificationCode);

        Task<ServiceResult> ResetPasswordAsync(RenewPasswordRequest request);
    }
}
