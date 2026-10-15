using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class RegisterUserViewModel
    {
        [Display(Name = "Ad")]
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Soyad")]
        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Display(Name = "E-posta")]
        [Required, MaxLength(256), EmailAddress]
        public string Email { get; set; } = null!;

        [Display(Name = "Şifre")]
        [Required, MinLength(8), MaxLength(128), DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Şifre tekrarı")]
        [Required, MinLength(8), MaxLength(128), DataType(DataType.Password), Compare(nameof(Password))]
        public string PasswordConfirm { get; set; } = null!;
    }
}