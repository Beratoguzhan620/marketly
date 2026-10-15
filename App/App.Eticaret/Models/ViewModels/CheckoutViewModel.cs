using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Display(Name = "Teslimat adresi")]
        [Required(ErrorMessage = "Teslimat adresini yazın."), MinLength(10, ErrorMessage = "Adres en az 10 karakter olmalı."), MaxLength(250), DataType(DataType.MultilineText)]
        public string Address { get; set; } = null!;

        [MaxLength(32)]
        public string? CouponCode { get; set; }
    }

    /// <summary>ViewData keys shared by the checkout page and the order summary partial.</summary>
    public static class CheckoutViewData
    {
        public const string CouponQuote = "CouponQuote";
        public const string CouponError = "CouponError";
        public const string CouponInput = "CouponInput";
    }
}
