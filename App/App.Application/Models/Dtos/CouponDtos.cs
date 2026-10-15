namespace App.Application.Models.Dtos
{
    public class CouponQuoteDto
    {
        public string Code { get; init; } = null!;
        public byte DiscountRate { get; init; }
        public decimal Subtotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal Total => Subtotal - DiscountAmount;
    }

    public class AdminCouponRowDto
    {
        public int Id { get; init; }
        public string Code { get; init; } = null!;
        public byte DiscountRate { get; init; }
        public decimal? MinimumOrderTotal { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public int? UsageLimit { get; init; }
        public int UsedCount { get; init; }
        public bool Enabled { get; init; }
    }
}
