using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace App.Application.Services
{
    public class AuthService(DataRepository<UserEntity> userRepo, DataRepository<RoleEntity> roleRepo) : IAuthService
    {
        private const string BuyerRoleName = "buyer";

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
                Password = request.Password,
                RoleId = buyerRole.Id,
            };

            await userRepo.AddAsync(user);

            return ServiceResult.Ok("Kayıt işlemi başarılı. Giriş yapabilirsiniz.");
        }

        public async Task<ServiceResult<AuthUserDto>> ValidateLoginAsync(LoginRequest request)
        {
            var user = await userRepo.GetAll()
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (user is null)
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

        public async Task<ServiceResult> RequestPasswordResetAsync(string email)
        {
            var user = await userRepo.GetAll().FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                return ServiceResult.NotFound("Kullanıcı bulunamadı.");
            }

            var resetPasswordToken = Guid.NewGuid().ToString("n");
            user.ResetPasswordToken = resetPasswordToken;
            await userRepo.UpdateAsync(user);

            await SendResetPasswordEmailAsync(user);

            return ServiceResult.Ok("Şifre sıfırlama maili gönderildi. Lütfen e-posta adresinizi kontrol edin.");
        }

        public async Task<ServiceResult<bool>> VerifyResetTokenAsync(string verificationCode)
        {
            if (string.IsNullOrEmpty(verificationCode))
            {
                return ServiceResult<bool>.ValidationError("Geçersiz doğrulama kodu.");
            }

            var exists = await userRepo.GetAll().AnyAsync(u => u.ResetPasswordToken == verificationCode);

            return exists
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.NotFound("Geçersiz veya süresi dolmuş doğrulama kodu.");
        }

        public async Task<ServiceResult> ResetPasswordAsync(RenewPasswordRequest request)
        {
            var user = await userRepo.GetAll().FirstOrDefaultAsync(u => u.ResetPasswordToken == request.VerificationCode);

            if (user is null)
            {
                return ServiceResult.NotFound("Geçersiz veya süresi dolmuş doğrulama kodu.");
            }

            user.Password = request.Password;
            user.ResetPasswordToken = null;

            await userRepo.UpdateAsync(user);

            return ServiceResult.Ok("Şifreniz başarıyla güncellendi. Giriş yapabilirsiniz.");
        }

        private static async Task SendResetPasswordEmailAsync(UserEntity user)
        {
            // Gönderici mail bilgileri güncellenmeli
            const string host = "smtp.gmail.com";
            const int port = 587;
            const string from = "mail";
            const string password = "şifre";

            using SmtpClient client = new(host, port)
            {
                Credentials = new NetworkCredential(from, password)
            };

            MailMessage mail = new()
            {
                From = new MailAddress(from),
                Subject = "Şifre Sıfırlama",
                Body = $"Merhaba {user.FirstName}, <br> Şifrenizi sıfırlamak için <a href='https://localhost:5001/renew-password/{user.ResetPasswordToken}'>tıklayınız</a>.",
                IsBodyHtml = true,
            };

            mail.To.Add(user.Email);

            await client.SendMailAsync(mail);
        }
    }
}
