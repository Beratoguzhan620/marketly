namespace App.Application.Models.Requests
{
    public class CouponCreateRequest
    {
        public string Code { get; set; } = null!;
        public byte DiscountRate { get; set; }
        public decimal? MinimumOrderTotal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}
