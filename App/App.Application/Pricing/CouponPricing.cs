using App.Data.Entities;

namespace App.Application.Pricing
{
    /// <summary>Rules for cart-wide coupon codes, kept free of EF so they are easy to test.</summary>
    public static class CouponPricing
    {
        public const int MaxCodeLength = 32;

        public static string Normalize(string? code) => (code ?? string.Empty).Trim().ToUpperInvariant();

        /// <summary>Returns an error message, or null when the coupon can be used for this subtotal.</summary>
        public static string? Check(CouponEntity? coupon, decimal subtotal, DateTime utcNow)
        {
            if (coupon is null || !coupon.Enabled)
                return "Bu indirim kodu geçerli değil.";

            if (utcNow < coupon.StartDate)
                return "Bu indirim kodu henüz kullanıma açılmadı.";

            if (utcNow > coupon.EndDate)
                return "Bu indirim kodunun süresi dolmuş.";

            if (coupon.UsageLimit is int limit && coupon.UsedCount >= limit)
                return "Bu indirim kodunun kullanım hakkı dolmuş.";

            if (coupon.MinimumOrderTotal is decimal minimum && subtotal < minimum)
                return $"Bu kod en az {minimum.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("tr-TR"))} ₺ tutarındaki siparişlerde geçerli.";

            return null;
        }

        public static decimal Discount(decimal subtotal, byte rate) =>
            rate is > 0 and <= 100
                ? Math.Round(subtotal * rate / 100m, 2, MidpointRounding.AwayFromZero)
                : 0m;
    }
}
