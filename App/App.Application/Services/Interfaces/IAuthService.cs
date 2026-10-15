using App.Application.Models.Requests;
using App.Application.Models.Dtos;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult> RegisterAsync(RegisterRequest request);

        Task<ServiceResult<AuthUserDto>> ValidateLoginAsync(LoginRequest request);

        /// <param name="requestBaseUrl">Scheme and host of the current request, used when Email:StorefrontUrl is empty.</param>
        Task<ServiceResult<PasswordResetRequestDto>> RequestPasswordResetAsync(string email, string requestBaseUrl);

        Task<ServiceResult<bool>> VerifyResetTokenAsync(string verificationCode);

        Task<ServiceResult> ResetPasswordAsync(RenewPasswordRequest request);
    }
}
