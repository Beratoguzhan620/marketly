using App.Application.Models.Dtos;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class CartService(DataRepository<CartItemEntity> cartItemRepo, DataRepository<ProductEntity> productRepo) : ICartService
    {
        public async Task<CartDto> GetCartAsync(int userId)
        {
            var items = await cartItemRepo.GetAll()
                .Include(ci => ci.Product.Images)
                .Where(ci => ci.UserId == userId)
                .Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductImage = ci.Product.Images.Count != 0 ? ci.Product.Images.First().Url : null,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    StockAmount = ci.Product.StockAmount
                })
                .ToListAsync();

            return new CartDto { Items = items };
        }

        public async Task<ServiceResult> AddProductAsync(int userId, int productId)
        {
            var product = await productRepo.GetByIdAsync(productId);

            if (product is null || !product.Enabled)
            {
                return ServiceResult.NotFound("Ürün bulunamadı.");
            }

            var cartItem = await cartItemRepo.GetAll()
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            var newQuantity = (cartItem?.Quantity ?? 0) + 1;

            if (newQuantity > product.StockAmount)
            {
                return ServiceResult.ValidationError("Yeterli stok bulunmuyor.");
            }

            if (cartItem is not null)
            {
                cartItem.Quantity = (byte)newQuantity;
                await cartItemRepo.UpdateAsync(cartItem);
            }
            else
            {
                await cartItemRepo.AddAsync(new CartItemEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                });
            }

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> RemoveItemAsync(int userId, int cartItemId)
        {
            var cartItem = await cartItemRepo.GetAll()
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.Id == cartItemId);

            if (cartItem is null)
            {
                return ServiceResult.NotFound("Sepet ürünü bulunamadı.");
            }

            await cartItemRepo.DeleteAsync(cartItemId);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<CartItemDto>> UpdateItemQuantityAsync(int userId, int cartItemId, byte quantity)
        {
            var cartItem = await cartItemRepo.GetAll()
                .Include(ci => ci.Product.Images)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.Id == cartItemId);

            if (cartItem is null)
            {
                return ServiceResult<CartItemDto>.NotFound("Sepet ürünü bulunamadı.");
            }

            if (quantity < 1)
            {
                return ServiceResult<CartItemDto>.ValidationError("Adet en az 1 olmalıdır.");
            }

            if (quantity > cartItem.Product.StockAmount)
            {
                return ServiceResult<CartItemDto>.ValidationError("Girilen adet stok miktarından fazla olamaz.");
            }

            cartItem.Quantity = quantity;
            cartItem = (await cartItemRepo.UpdateAsync(cartItem))!;

            return ServiceResult<CartItemDto>.Ok(new CartItemDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                ProductImage = cartItem.Product.Images.Count != 0 ? cartItem.Product.Images.First().Url : null,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product.Price,
                StockAmount = cartItem.Product.StockAmount
            });
        }
    }
}
