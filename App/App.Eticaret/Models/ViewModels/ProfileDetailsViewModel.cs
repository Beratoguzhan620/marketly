using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class ProfileDetailsViewModel
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

        [Display(Name = "Yeni şifre")]
        [MinLength(4), DataType(DataType.Password)]
        public string? Password { get; set; }

        /// <summary>Display only; never bound from the form.</summary>
        public bool HasSellerRequest { get; set; }

        /// <summary>Display only; never bound from the form.</summary>
        public string RoleName { get; set; } = string.Empty;
    }
}