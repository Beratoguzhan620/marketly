using App.Application.Security;
using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class UserService(DataRepository<UserEntity> userRepo, DataRepository<RoleEntity> roleRepo) : IUserService
    {
        private const string SellerRoleName = "seller";

        public async Task<ServiceResult<UserProfileDto>> GetProfileAsync(int userId)
        {
            var user = await userRepo.GetAll()
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                })
                .FirstOrDefaultAsync();

            return user is null
                ? ServiceResult<UserProfileDto>.NotFound("Kullanıcı bulunamadı.")
                : ServiceResult<UserProfileDto>.Ok(user);
        }

        public async Task<ServiceResult> UpdateProfileAsync(int userId, UserUpdateRequest request)
        {
            var user = await userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ServiceResult.NotFound("Kullanıcı bulunamadı.");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            if (!string.IsNullOrWhiteSpace(request.Password) && request.Password != "******")
            {
                user.Password = PasswordHasher.Hash(request.Password);
            }

            await userRepo.UpdateAsync(user);

            return ServiceResult.Ok("Profiliniz başarıyla güncellendi.");
        }

        public async Task<ServiceResult<List<UserListDto>>> GetUsersAsync()
        {
            var users = await userRepo.GetAll()
                .Where(u => u.RoleId != 1) // admin kullanıcılar listelenmez
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.Name,
                    Enabled = u.Enabled,
                    HasSellerRequest = u.HasSellerRequest
                })
                .ToListAsync();

            return ServiceResult<List<UserListDto>>.Ok(users);
        }

        public async Task<ServiceResult> SetUserEnabledAsync(int userId, bool enabled)
        {
            var user = await userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ServiceResult.NotFound("Kullanıcı bulunamadı.");
            }

            user.Enabled = enabled;
            await userRepo.UpdateAsync(user);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> RequestSellerRoleAsync(int userId)
        {
            var user = await userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ServiceResult.NotFound("Kullanıcı bulunamadı.");
            }

            if (user.HasSellerRequest)
            {
                return ServiceResult.ValidationError("Zaten bekleyen bir satıcı olma talebiniz bulunuyor.");
            }

            user.HasSellerRequest = true;
            await userRepo.UpdateAsync(user);

            return ServiceResult.Ok("Satıcı olma talebiniz alındı. Onaylandığında bilgilendirileceksiniz.");
        }

        public async Task<ServiceResult> ApproveSellerRequestAsync(int userId)
        {
            var user = await userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ServiceResult.NotFound("Kullanıcı bulunamadı.");
            }

            if (!user.HasSellerRequest)
            {
                return ServiceResult.ValidationError("Kullanıcının bekleyen bir satıcı olma talebi yok.");
            }

            var sellerRole = await roleRepo.GetAll().FirstOrDefaultAsync(r => r.Name == SellerRoleName);

            if (sellerRole is null)
            {
                return ServiceResult.Fail("Satıcı rolü bulunamadı.");
            }

            user.HasSellerRequest = false;
            user.RoleId = sellerRole.Id;

            await userRepo.UpdateAsync(user);

            return ServiceResult.Ok("Satıcı olma talebi onaylandı.");
        }
    }
}
