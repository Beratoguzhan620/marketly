using App.Data.Entities;

namespace App.Application.Pricing
{
    /// <summary>
    /// Single place that decides what a product costs right now, so the storefront,
    /// the cart and the order use the same number.
    /// </summary>
    public static class ProductPricing
    {
        public static bool IsActive(DiscountEntity? discount, DateTime utcNow) =>
            discount is not null
            && discount.Enabled
            && discount.DiscountRate > 0
            && discount.StartDate <= utcNow
            && discount.EndDate >= utcNow;

        public static byte? ActiveRate(DiscountEntity? discount, DateTime utcNow) =>
            IsActive(discount, utcNow) ? discount!.DiscountRate : null;

        public static decimal Apply(decimal price, byte? discountRate) =>
            discountRate is > 0 and <= 100
                ? Math.Round(price * (100 - discountRate.Value) / 100m, 2, MidpointRounding.AwayFromZero)
                : price;
    }
}
