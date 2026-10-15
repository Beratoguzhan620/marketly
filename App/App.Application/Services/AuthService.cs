using App.Application.Models.Dtos;
using App.Application.Options;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Security;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace App.Application.Services
{
    public class AuthService(
        DataRepository<UserEntity> userRepo,
        DataRepository<RoleEntity> roleRepo,
        IOptions<EmailOptions> emailOptions,
        TimeProvider timeProvider,
        ILogger<AuthService> logger) : IAuthService
    {
        private const string BuyerRoleName = "buyer";
        private const string GenericResetMessage = "Bu e-posta adresiyle kayıtlı bir hesap varsa şifre sıfırlama bağlantısını gönderdik. Gelen kutunuzu ve istenmeyen klasörünü kontrol edin.";
        private const string InvalidResetLinkMessage = "Bu bağlantı geçersiz ya da süresi dolmuş. Yeni bir bağlantı isteyin.";

        public static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromHours(1);
        public static readonly TimeSpan ResendCooldown = TimeSpan.FromMinutes(1);

        public async Task<ServiceResult> RegisterAsync(RegisterRequest request)
        {
            var emailInUse = await userRepo.GetAll()
                .AnyAsync(u => u.Email == request.Email);

            if (emailInUse)
            {
                return ServiceResult.ValidationError("Bu email adresi ile daha önce kayıt oluşturulmuş.");
            }

            var buyerRole = await roleRepo.GetAll().FirstOrDefaultAsync(r => r.Name == BuyerRoleName);

            if (buyerRole is null)
            {
                return ServiceResult.Fail("Kullanıcı rolü bulunamadı.");
            }

            var user = new UserEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = PasswordHasher.Hash(request.Password),
                RoleId = buyerRole.Id,
            };

            await userRepo.AddAsync(user);

            return ServiceResult.Ok("Kayıt işlemi başarılı. Giriş yapabilirsiniz.");
        }

        public async Task<ServiceResult<AuthUserDto>> ValidateLoginAsync(LoginRequest request)
        {
            var user = await userRepo.GetAll()
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == request.Email);

            if (user is null || !PasswordHasher.Verify(request.Password, user.Password))
            {
                return ServiceResult<AuthUserDto>.ValidationError("Kullanıcı adı veya şifre hatalı.");
            }

            if (!user.Enabled)
            {
                return ServiceResult<AuthUserDto>.Forbidden("Hesabınız pasif durumda. Lütfen yönetici ile iletişime geçin.");
            }

            var dto = new AuthUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = user.Role.Name,
            };

            return ServiceResult<AuthUserDto>.Ok(dto);
        }

        public async Task<ServiceResult<PasswordResetRequestDto>> RequestPasswordResetAsync(string email, string requestBaseUrl)
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var normalized = (email ?? string.Empty).Trim();
            var user = await userRepo.GetAll().FirstOrDefaultAsync(u => u.Email == normalized);

            // Same answer whether or not the address is registered, so the form cannot be used
            // to find out who has an account.
            var response = new PasswordResetRequestDto();

            if (user is null || !user.Enabled)
            {
                return ServiceResult<PasswordResetRequestDto>.Ok(response, GenericResetMessage);
            }

            // A link was sent less than a minute ago: do not send another one.
            if (user.ResetPasswordTokenExpiresAt is DateTime expires && expires - ResetTokenLifetime > now - ResendCooldown)
            {
                return ServiceResult<PasswordResetRequestDto>.Ok(response, GenericResetMessage);
            }

            var token = ResetToken.Create();
            user.ResetPasswordToken = ResetToken.Hash(token);
            user.ResetPasswordTokenExpiresAt = now + ResetTokenLifetime;
            await userRepo.UpdateAsync(user);

            var options = emailOptions.Value;
            var baseUrl = string.IsNullOrWhiteSpace(options.StorefrontUrl) ? requestBaseUrl : options.StorefrontUrl;
            var resetLink = $"{baseUrl.TrimEnd('/')}/renew-password/{token}";

            var sent = await TrySendResetPasswordEmailAsync(user, resetLink);
            if (!sent)
            {
                // No SMTP in development: hand the link back so it can be shown on the page.
                response.UndeliveredLink = resetLink;
            }

            return ServiceResult<PasswordResetRequestDto>.Ok(response, GenericResetMessage);
        }

        public async Task<ServiceResult<bool>> VerifyResetTokenAsync(string verificationCode)
        {
            var user = await FindUserByResetTokenAsync(verificationCode);

            return user is not null
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.NotFound(InvalidResetLinkMessage);
        }

        public async Task<ServiceResult> ResetPasswordAsync(RenewPasswordRequest request)
        {
            var user = await FindUserByResetTokenAsync(request.VerificationCode);

            if (user is null)
            {
                return ServiceResult.NotFound(InvalidResetLinkMessage);
            }

            user.Password = PasswordHasher.Hash(request.Password);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiresAt = null;

            await userRepo.UpdateAsync(user);

            return ServiceResult.Ok("Şifreniz güncellendi. Yeni şifrenizle giriş yapabilirsiniz.");
        }

        private async Task<UserEntity?> FindUserByResetTokenAsync(string? token)
        {
            if (!ResetToken.LooksValid(token))
            {
                return null;
            }

            var hash = ResetToken.Hash(token!);
            var now = timeProvider.GetUtcNow().UtcDateTime;

            return await userRepo.GetAll()
                .FirstOrDefaultAsync(u => u.ResetPasswordToken == hash && u.ResetPasswordTokenExpiresAt > now && u.Enabled);
        }

        private async Task<bool> TrySendResetPasswordEmailAsync(UserEntity user, string resetLink)
        {
            var options = emailOptions.Value;

            if (!options.IsConfigured)
            {
                logger.LogWarning("SMTP is not configured; password reset e-mail for user {UserId} was not sent.", user.Id);
                return false;
            }

            try
            {
                using SmtpClient client = new(options.Host, options.Port)
                {
                    EnableSsl = options.EnableSsl,
                    Credentials = new NetworkCredential(options.Username, options.Password),
                };

                using MailMessage mail = new()
                {
                    From = new MailAddress(options.FromAddress, options.FromName),
                    Subject = "Marketly şifre sıfırlama",
                    Body = $"Merhaba {WebUtility.HtmlEncode(user.FirstName)},<br><br>"
                        + $"Şifrenizi yenilemek için <a href=\"{WebUtility.HtmlEncode(resetLink)}\">bu bağlantıya</a> tıklayın. "
                        + "Bağlantı 1 saat geçerlidir ve yalnızca bir kez kullanılabilir.<br><br>"
                        + "Bu isteği siz yapmadıysanız e-postayı yok sayabilirsiniz.",
                    IsBodyHtml = true,
                };

                mail.To.Add(user.Email);
                await client.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex) when (ex is SmtpException or InvalidOperationException or FormatException)
            {
                logger.LogError(ex, "Password reset e-mail for user {UserId} could not be sent.", user.Id);
                return false;
            }
        }
    }
}
