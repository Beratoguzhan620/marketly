using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class NewContactFormMessageViewModel
    {
        [Display(Name = "Ad soyad")]
        [Required, MinLength(2), MaxLength(100)]
        public string Name { get; set; } = null!;

        [Display(Name = "E-posta")]
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = null!;

        [Display(Name = "Mesaj")]
        [Required, MinLength(10), MaxLength(1000)]
        public string Message { get; set; } = null!;
    }
}