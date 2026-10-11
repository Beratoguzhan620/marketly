using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();

        Task<ServiceResult<CategoryDto>> GetByIdAsync(int categoryId);

        Task<ServiceResult> CreateAsync(CategoryCreateRequest request);

        Task<ServiceResult> UpdateAsync(int categoryId, CategoryUpdateRequest request);

        Task<ServiceResult> DeleteAsync(int categoryId);
    }
}
