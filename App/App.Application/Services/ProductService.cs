using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Application.Services
{
    public class ProductService(
        DataRepository<ProductEntity> productRepo,
        DataRepository<ProductImageEntity> productImageRepo,
        IProductCommentService productCommentService,
        IFileApiClient fileApiClient,
        ILogger<ProductService> logger) : IProductService
    {
        public async Task<List<ProductListDto>> GetListingAsync(string? searchTerm = null)
        {
            var query = productRepo.GetAll().Where(p => p.Enabled);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            return await query
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    DiscountPercentage = p.Discount == null ? null : p.Discount.DiscountRate,
                    ImageUrl = p.Images.Count != 0 ? p.Images.First().Url : null
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<ProductDetailDto>> GetDetailAsync(int productId)
        {
            var product = await productRepo.GetAll()
                .Where(p => p.Enabled && p.Id == productId)
                .Select(p => new ProductDetailDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountRate = p.Discount == null ? null : p.Discount.DiscountRate,
                    Description = p.Description,
                    StockAmount = p.StockAmount,
                    SellerName = p.Seller.FirstName + " " + p.Seller.LastName,
                    CategoryName = p.Category.Name,
                    CategoryId = p.CategoryId,
                    ImageUrls = p.Images.Select(i => i.Url).ToArray(),
                })
                .FirstOrDefaultAsync();

            if (product is null)
            {
                return ServiceResult<ProductDetailDto>.NotFound();
            }

            // Onaylanmış yorumlar başka bir servisin sorumluluğunda olduğu için ilgili servisten alınır.
            product.Comments = await productCommentService.GetApprovedCommentsAsync(productId);

            return ServiceResult<ProductDetailDto>.Ok(product);
        }

        public async Task<ServiceResult> CreateAsync(int sellerId, ProductCreateRequest request)
        {
            var uploadResult = await UploadImagesAsync(request.Images);

            if (!uploadResult.Success || uploadResult.Data is null)
            {
                return ServiceResult.ValidationError(uploadResult.Message ?? "Ürün görsellerinden biri yüklenemedi.");
            }

            var uploadedFiles = uploadResult.Data;

            await using var transaction = await productRepo.BeginTransactionAsync();

            try
            {
                var product = new ProductEntity
                {
                    SellerId = sellerId,
                    CategoryId = request.CategoryId,
                    DiscountId = request.DiscountId,
                    Name = request.Name,
                    Price = request.Price,
                    Description = request.Description,
                    StockAmount = request.StockAmount,
                };

                product = await productRepo.AddAsync(product);

                foreach (var uploadedFile in uploadedFiles)
                {
                    await productImageRepo.AddAsync(new ProductImageEntity
                    {
                        ProductId = product.Id,
                        Url = uploadedFile.Url,
                        FileKey = uploadedFile.FileKey
                    });
                }

                await transaction.CommitAsync();

                return ServiceResult.Ok("Ürün başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ürün oluşturulurken bir hata oluştu. SellerId: {SellerId}", sellerId);
                await transaction.RollbackAsync();

                // Ürün kaydı başarısız olduğu için daha önce File API'ye yüklenmiş dosyalar telafi amacıyla silinir.
                await CompensateUploadedFilesAsync(uploadedFiles);

                return ServiceResult.Fail("Ürün oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        public async Task<ServiceResult<ProductEditDto>> GetForEditAsync(int sellerId, int productId)
        {
            var product = await productRepo.GetAll()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product is null)
            {
                return ServiceResult<ProductEditDto>.NotFound("Ürün bulunamadı.");
            }

            if (product.SellerId != sellerId)
            {
                return ServiceResult<ProductEditDto>.Forbidden();
            }

            return ServiceResult<ProductEditDto>.Ok(new ProductEditDto
            {
                CategoryId = product.CategoryId,
                DiscountId = product.DiscountId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                StockAmount = product.StockAmount,
                Images = product.Images.Select(i => new ProductImageDto { Id = i.Id, Url = i.Url }).ToList()
            });
        }

        public async Task<ServiceResult> UpdateAsync(int sellerId, int productId, ProductUpdateRequest request)
        {
            var product = await productRepo.GetByIdAsync(productId);

            if (product is null)
            {
                return ServiceResult.NotFound("Ürün bulunamadı.");
            }

            if (product.SellerId != sellerId)
            {
                return ServiceResult.Forbidden("Yalnızca kendi ürünlerinizi güncelleyebilirsiniz.");
            }

            var uploadResult = await UploadImagesAsync(request.NewImages);

            if (!uploadResult.Success || uploadResult.Data is null)
            {
                return ServiceResult.ValidationError(uploadResult.Message ?? "Ürün görsellerinden biri yüklenemedi.");
            }

            var uploadedFiles = uploadResult.Data;

            await using var transaction = await productRepo.BeginTransactionAsync();

            try
            {
                product.CategoryId = request.CategoryId;
                product.DiscountId = request.DiscountId;
                product.Name = request.Name;
                product.Price = request.Price;
                product.Description = request.Description;
                product.StockAmount = request.StockAmount;

                await productRepo.UpdateAsync(product);

                foreach (var uploadedFile in uploadedFiles)
                {
                    await productImageRepo.AddAsync(new ProductImageEntity
                    {
                        ProductId = product.Id,
                        Url = uploadedFile.Url,
                        FileKey = uploadedFile.FileKey
                    });
                }

                await transaction.CommitAsync();

                return ServiceResult.Ok("Ürün başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ürün güncellenirken bir hata oluştu. ProductId: {ProductId}", productId);
                await transaction.RollbackAsync();

                await CompensateUploadedFilesAsync(uploadedFiles);

                return ServiceResult.Fail("Ürün güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        public async Task<ServiceResult> DeactivateAsync(int sellerId, int productId)
        {
            var product = await productRepo.GetByIdAsync(productId);

            if (product is null)
            {
                return ServiceResult.NotFound("Ürün bulunamadı.");
            }

            if (product.SellerId != sellerId)
            {
                return ServiceResult.Forbidden("Yalnızca kendi ürünlerinizi pasif hale getirebilirsiniz.");
            }

            product.Enabled = false;
            await productRepo.UpdateAsync(product);

            return ServiceResult.Ok("Ürün başarıyla pasif hale getirildi.");
        }

        public async Task<ServiceResult> DeleteProductImageAsync(int sellerId, int productId, int imageId)
        {
            var product = await productRepo.GetByIdAsync(productId);

            if (product is null)
            {
                return ServiceResult.NotFound("Ürün bulunamadı.");
            }

            // Bir seller kullanıcısının başka bir seller'a ait ürünün görselini silememesi için kontrol edilir.
            if (product.SellerId != sellerId)
            {
                return ServiceResult.Forbidden("Yalnızca kendi ürünlerinizin görsellerini silebilirsiniz.");
            }

            return await DeleteProductImageInternalAsync(productId, imageId);
        }

        public async Task<List<AdminProductListDto>> GetAllForAdminAsync()
        {
            return await productRepo.GetAll()
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new AdminProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    SellerName = p.Seller.FirstName + " " + p.Seller.LastName,
                    Price = p.Price,
                    Enabled = p.Enabled,
                    ImageUrl = p.Images.Count != 0 ? p.Images.First().Url : null
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<AdminProductImagesDto>> GetProductImagesForAdminAsync(int productId)
        {
            var product = await productRepo.GetAll()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product is null)
            {
                return ServiceResult<AdminProductImagesDto>.NotFound("Ürün bulunamadı.");
            }

            return ServiceResult<AdminProductImagesDto>.Ok(new AdminProductImagesDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Images = product.Images.Select(i => new ProductImageDto { Id = i.Id, Url = i.Url }).ToList()
            });
        }

        public async Task<ServiceResult> AddProductImagesAsAdminAsync(int productId, List<FileUploadRequest> images)
        {
            var product = await productRepo.GetByIdAsync(productId);

            if (product is null)
            {
                return ServiceResult.NotFound("Ürün bulunamadı.");
            }

            if (images.Count == 0)
            {
                return ServiceResult.ValidationError("En az bir görsel seçmelisiniz.");
            }

            var uploadResult = await UploadImagesAsync(images);

            if (!uploadResult.Success || uploadResult.Data is null)
            {
                return ServiceResult.ValidationError(uploadResult.Message ?? "Görsellerden biri yüklenemedi.");
            }

            var uploadedFiles = uploadResult.Data;

            try
            {
                foreach (var uploadedFile in uploadedFiles)
                {
                    await productImageRepo.AddAsync(new ProductImageEntity
                    {
                        ProductId = product.Id,
                        Url = uploadedFile.Url,
                        FileKey = uploadedFile.FileKey
                    });
                }

                return ServiceResult.Ok("Görseller başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Admin tarafından görsel eklenirken bir hata oluştu. ProductId: {ProductId}", productId);

                await CompensateUploadedFilesAsync(uploadedFiles);

                return ServiceResult.Fail("Görseller eklenirken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        public Task<ServiceResult> DeleteProductImageAsAdminAsync(int productId, int imageId) =>
            DeleteProductImageInternalAsync(productId, imageId);

        private async Task<ServiceResult> DeleteProductImageInternalAsync(int productId, int imageId)
        {
            var image = await productImageRepo.GetByIdAsync(imageId);

            if (image is null || image.ProductId != productId)
            {
                return ServiceResult.NotFound("Görsel bulunamadı.");
            }

            if (!string.IsNullOrWhiteSpace(image.FileKey))
            {
                var deleteResult = await fileApiClient.DeleteAsync(image.FileKey);

                if (!deleteResult.Success)
                {
                    return ServiceResult.Fail(deleteResult.Message ?? "Görsel silinirken bir hata oluştu.");
                }
            }

            await productImageRepo.DeleteAsync(imageId);

            return ServiceResult.Ok("Görsel başarıyla silindi.");
        }

        public async Task<List<MyProductDto>> GetMyProductsAsync(int sellerId)
        {
            return await productRepo.GetAll()
                .Where(p => p.SellerId == sellerId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new MyProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Stock = p.StockAmount,
                    HasDiscount = p.DiscountId != null,
                    Enabled = p.Enabled,
                    CreatedAt = p.CreatedAt,
                })
                .ToListAsync();
        }

        public async Task<bool> IsAvailableForPurchaseAsync(int productId, byte requestedQuantity)
        {
            var product = await productRepo.GetByIdAsync(productId);

            if (product is null || !product.Enabled)
            {
                return false;
            }

            return product.StockAmount >= requestedQuantity;
        }

        /// <summary>
        /// Görselleri File API'ye sırayla yükler. Herhangi biri başarısız olursa, o ana kadar
        /// başarıyla yüklenmiş olanlar telafi amacıyla silinir ve anlaşılır bir hata döndürülür.
        /// </summary>
        private async Task<ServiceResult<List<UploadedFileDto>>> UploadImagesAsync(List<FileUploadRequest> images)
        {
            var uploadedFiles = new List<UploadedFileDto>();

            foreach (var image in images)
            {
                var result = await fileApiClient.UploadAsync(image);

                if (!result.Success || result.Data is null)
                {
                    logger.LogWarning("Ürün görseli yüklenemedi: {FileName}, Sebep: {Message}", image.FileName, result.Message);

                    await CompensateUploadedFilesAsync(uploadedFiles);

                    return ServiceResult<List<UploadedFileDto>>.ValidationError(
                        $"'{image.FileName}' adlı görsel yüklenemedi: {result.Message}");
                }

                uploadedFiles.Add(result.Data);
            }

            return ServiceResult<List<UploadedFileDto>>.Ok(uploadedFiles);
        }

        private async Task CompensateUploadedFilesAsync(List<UploadedFileDto> uploadedFiles)
        {
            foreach (var uploadedFile in uploadedFiles)
            {
                try
                {
                    await fileApiClient.DeleteAsync(uploadedFile.FileKey);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Telafi silme işlemi başarısız oldu. FileKey: {FileKey}", uploadedFile.FileKey);
                }
            }
        }
    }
}
