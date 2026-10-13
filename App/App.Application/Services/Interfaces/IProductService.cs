using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductListDto>> GetListingAsync(ProductListingQuery query);

        Task<ServiceResult<ProductDetailDto>> GetDetailAsync(int productId);

        Task<ServiceResult> CreateAsync(int sellerId, ProductCreateRequest request);

        Task<ServiceResult<ProductEditDto>> GetForEditAsync(int sellerId, int productId);

        Task<ServiceResult> UpdateAsync(int sellerId, int productId, ProductUpdateRequest request);

        Task<ServiceResult> DeactivateAsync(int sellerId, int productId);

        Task<ServiceResult> DeleteProductImageAsync(int sellerId, int productId, int imageId);

        /// <summary>
        /// Admin panelindeki ürün görsel yönetimi için kullanılan, seller sahiplik kontrolü
        /// yapmayan (admin herhangi bir ürünü yönetebilir) metotlar.
        /// </summary>
        Task<List<AdminProductListDto>> GetAllForAdminAsync();

        Task<ServiceResult<AdminProductImagesDto>> GetProductImagesForAdminAsync(int productId);

        Task<ServiceResult> AddProductImagesAsAdminAsync(int productId, List<FileUploadRequest> images);

        Task<ServiceResult> DeleteProductImageAsAdminAsync(int productId, int imageId);

        Task<List<MyProductDto>> GetMyProductsAsync(int sellerId);

        Task<bool> IsAvailableForPurchaseAsync(int productId, byte requestedQuantity);
    }
}
