using App.Application.Pricing;
using App.Data.Entities;

namespace App.Application.Tests;

public class CouponPricingTests
{
    private static readonly DateTime Now = new(2026, 5, 10, 12, 0, 0, DateTimeKind.Utc);

    private static CouponEntity Coupon(Action<CouponEntity>? change = null)
    {
        var coupon = new CouponEntity
        {
            Code = "BAHAR25",
            DiscountRate = 25,
            StartDate = Now.AddDays(-1),
            EndDate = Now.AddDays(10),
            Enabled = true,
        };
        change?.Invoke(coupon);
        return coupon;
    }

    [Theory]
    [InlineData(" bahar25 ", "BAHAR25")]
    [InlineData(null, "")]
    public void Codes_are_trimmed_and_upper_cased(string? input, string expected) =>
        Assert.Equal(expected, CouponPricing.Normalize(input));

    [Fact]
    public void A_valid_coupon_has_no_error() =>
        Assert.Null(CouponPricing.Check(Coupon(), 100m, Now));

    [Fact]
    public void An_unknown_or_disabled_coupon_is_rejected()
    {
        Assert.NotNull(CouponPricing.Check(null, 100m, Now));
        Assert.NotNull(CouponPricing.Check(Coupon(c => c.Enabled = false), 100m, Now));
    }

    [Fact]
    public void A_coupon_only_works_between_its_dates()
    {
        Assert.Contains("henüz", CouponPricing.Check(Coupon(c => c.StartDate = Now.AddHours(1)), 100m, Now));
        Assert.Contains("süresi dolmuş", CouponPricing.Check(Coupon(c => c.EndDate = Now.AddSeconds(-1)), 100m, Now));
    }

    [Fact]
    public void A_used_up_coupon_is_rejected()
    {
        Assert.NotNull(CouponPricing.Check(Coupon(c => { c.UsageLimit = 3; c.UsedCount = 3; }), 100m, Now));
        Assert.Null(CouponPricing.Check(Coupon(c => { c.UsageLimit = 3; c.UsedCount = 2; }), 100m, Now));
    }

    [Fact]
    public void The_minimum_order_total_is_inclusive()
    {
        var coupon = Coupon(c => c.MinimumOrderTotal = 200m);

        Assert.NotNull(CouponPricing.Check(coupon, 199.99m, Now));
        Assert.Null(CouponPricing.Check(coupon, 200m, Now));
    }

    [Theory]
    [InlineData("552.65", 10, "55.27")]
    [InlineData("100", 25, "25")]
    [InlineData("0.05", 10, "0.01")]
    public void The_discount_is_rounded_to_kurus(string subtotal, byte rate, string expected) =>
        Assert.Equal(Dec(expected), CouponPricing.Discount(Dec(subtotal), rate));

    private static decimal Dec(string value) => decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
}
