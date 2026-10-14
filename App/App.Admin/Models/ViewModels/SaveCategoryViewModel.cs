using System.ComponentModel.DataAnnotations;

namespace App.Admin.Models.ViewModels
{
    public class SaveCategoryViewModel
    {
        [Display(Name = "Kategori adı")]
        [Required(ErrorMessage = "Kategori adı gereklidir."), MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = null!;

        [Display(Name = "Renk")]
        [Required(ErrorMessage = "Renk gereklidir."), MaxLength(30, ErrorMessage = "Renk en fazla 30 karakter olabilir.")]
        public string Color { get; set; } = null!;

        [Display(Name = "İkon")]
        [MaxLength(50, ErrorMessage = "İkon sınıfı en fazla 50 karakter olabilir.")]
        [RegularExpression("^(bi-[a-z0-9-]+)?$", ErrorMessage = "İkon, Bootstrap Icons sınıf adı olmalıdır (ör. bi-basket).")]
        public string? IconCssClass { get; set; } = string.Empty;
    }
}
