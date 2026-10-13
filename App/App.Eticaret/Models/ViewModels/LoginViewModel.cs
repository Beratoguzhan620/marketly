using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "E-posta")]
        [Required, MaxLength(256), EmailAddress]
        public string Email { get; set; } = null!;

        [Display(Name = "Şifre")]
        [Required, MinLength(4), DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}