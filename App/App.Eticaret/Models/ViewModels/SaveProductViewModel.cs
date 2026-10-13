using System.ComponentModel.DataAnnotations;
using App.Application.Models.Dtos;
using App.Eticaret.Validation;

namespace App.Eticaret.Models.ViewModels
{
    public class SaveProductViewModel
    {
        public int SellerId { get; set; }

        [Display(Name = "Kategori")]
        [Required]
        public int CategoryId { get; set; }

        [Display(Name = "İndirim")]
        public int? DiscountId { get; set; }

        [Display(Name = "Ürün adı")]
        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; } = null!;

        [Display(Name = "Fiyat")]
        [Required, Range(0.01, 1000000, ErrorMessage = "Fiyat 0,01 ile 1.000.000 arasında olmalıdır."), DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Açıklama")]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;

        [Display(Name = "Stok")]
        [Required, Range(1, 255)]
        public byte StockAmount { get; set; }

        [Display(Name = "Görseller")]
        [ValidImages]
        public IList<IFormFile> Images { get; set; } = [];

        /// <summary>
        /// Yalnızca Edit ekranında mevcut görselleri listelemek için kullanılır; POST edilmez.
        /// </summary>
        public List<ProductImageDto> CurrentImages { get; set; } = [];
    }
}