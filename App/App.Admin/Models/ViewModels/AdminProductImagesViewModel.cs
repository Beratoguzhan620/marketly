using App.Admin.Validation;
using App.Application.Models.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace App.Admin.Models.ViewModels
{
    public class AdminProductImagesViewModel
    {
        public int ProductId { get; set; }

        /// <summary>
        /// Yalnızca ekranda başlık olarak gösterilir; form tarafından POST edilmediği için
        /// model doğrulamasına dahil edilmez.
        /// </summary>
        [ValidateNever]
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Ürüne kayıtlı mevcut görseller; yalnızca ekranda listelemek için kullanılır, POST edilmez.
        /// </summary>
        [ValidateNever]
        public List<ProductImageDto> CurrentImages { get; set; } = [];

        [ValidImages]
        public IList<IFormFile> NewImages { get; set; } = [];
    }
}
