using App.Application.Models.Dtos;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);

        Task<ServiceResult> AddProductAsync(int userId, int productId);

        Task<ServiceResult> RemoveItemAsync(int userId, int cartItemId);

        Task<ServiceResult<CartItemDto>> UpdateItemQuantityAsync(int userId, int cartItemId, byte quantity);
    }
}
