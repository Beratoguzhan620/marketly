using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class CategoryService(DataRepository<CategoryEntity> categoryRepo, DataRepository<ProductEntity> productRepo) : ICategoryService
    {
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await categoryRepo.GetAll()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                    IconCssClass = c.IconCssClass,
                    ProductCount = productRepo.GetAll().Count(p => p.Enabled && p.CategoryId == c.Id),
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int categoryId)
        {
            var category = await categoryRepo.GetByIdAsync(categoryId);

            if (category is null)
            {
                return ServiceResult<CategoryDto>.NotFound("Kategori bulunamadı.");
            }

            return ServiceResult<CategoryDto>.Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color,
                IconCssClass = category.IconCssClass
            });
        }

        public async Task<ServiceResult> CreateAsync(CategoryCreateRequest request)
        {
            var category = new CategoryEntity
            {
                Name = request.Name,
                Color = request.Color,
                IconCssClass = request.IconCssClass ?? string.Empty,
            };

            await categoryRepo.AddAsync(category);

            return ServiceResult.Ok("Kategori başarıyla oluşturuldu.");
        }

        public async Task<ServiceResult> UpdateAsync(int categoryId, CategoryUpdateRequest request)
        {
            var category = await categoryRepo.GetByIdAsync(categoryId);

            if (category is null)
            {
                return ServiceResult.NotFound("Kategori bulunamadı.");
            }

            category.Name = request.Name;
            category.Color = request.Color;

            await categoryRepo.UpdateAsync(category);

            return ServiceResult.Ok("Kategori başarıyla güncellendi.");
        }

        public async Task<ServiceResult> DeleteAsync(int categoryId)
        {
            var category = await categoryRepo.GetByIdAsync(categoryId);

            if (category is null)
            {
                return ServiceResult.NotFound("Kategori bulunamadı.");
            }

            var hasProducts = await productRepo.GetAll().AnyAsync(p => p.CategoryId == categoryId);

            if (hasProducts)
            {
                return ServiceResult.ValidationError("Bu kategoriye bağlı ürünler bulunduğu için kategori silinemez.");
            }

            await categoryRepo.DeleteAsync(categoryId);

            return ServiceResult.Ok("Kategori başarıyla silindi.");
        }
    }
}
