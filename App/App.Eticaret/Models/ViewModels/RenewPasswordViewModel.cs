using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class RenewPasswordViewModel
    {
        [Display(Name = "Doğrulama kodu")]
        [Required]
        public string VerificationCode { get; set; } = null!;

        [Display(Name = "Yeni şifre")]
        [Required, MinLength(4), DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Şifre tekrarı")]
        [Required, MinLength(4), DataType(DataType.Password), Compare(nameof(Password))]
        public string PasswordConfirm { get; set; } = null!;
    }
}
