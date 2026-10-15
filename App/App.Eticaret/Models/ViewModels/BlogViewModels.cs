using App.Application.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class BlogCommentFormViewModel
    {
        [Display(Name = "Adınız")]
        [Required(ErrorMessage = "Adınızı yazın.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Adınız 2 ile 100 karakter arasında olmalı.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta adresinizi yazın.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Yorumunuz")]
        [Required(ErrorMessage = "Yorumunuzu yazın.")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Yorum 3 ile 1000 karakter arasında olmalı.")]
        public string Comment { get; set; } = string.Empty;
    }

    public class BlogDetailPageViewModel
    {
        public BlogPostDetailDto Post { get; init; } = null!;
        public BlogCommentFormViewModel Form { get; init; } = new();
    }
}
