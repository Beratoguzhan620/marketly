using System.ComponentModel.DataAnnotations;

namespace App.Admin.Models.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta gereklidir."), EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre gereklidir."), MinLength(4, ErrorMessage = "Şifre en az 4 karakter olmalıdır.")]
        public string Password { get; set; } = null!;
    }
}
