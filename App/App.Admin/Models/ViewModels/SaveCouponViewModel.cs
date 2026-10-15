using System.ComponentModel.DataAnnotations;

namespace App.Admin.Models.ViewModels
{
    public class SaveCouponViewModel
    {
        [Display(Name = "Kod")]
        [Required(ErrorMessage = "Kodu yazın.")]
        [RegularExpression("^[A-Za-z0-9]{4,32}$", ErrorMessage = "4-32 karakter; yalnızca harf ve rakam.")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "İndirim oranı (%)")]
        [Range(1, 90, ErrorMessage = "Oran %1 ile %90 arasında olmalı.")]
        public byte DiscountRate { get; set; } = 10;

        [Display(Name = "En az sepet tutarı (₺)")]
        [Range(typeof(decimal), "0", "1000000", ErrorMessage = "Geçerli bir tutar girin.")]
        public decimal? MinimumOrderTotal { get; set; }

        [Display(Name = "Başlangıç")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;

        [Display(Name = "Bitiş")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddMonths(1);

        [Display(Name = "Kullanım limiti")]
        [Range(1, 1_000_000, ErrorMessage = "Limit en az 1 olmalı.")]
        public int? UsageLimit { get; set; }
    }
}
