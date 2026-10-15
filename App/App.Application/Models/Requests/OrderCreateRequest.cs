namespace App.Application.Models.Requests
{
    public class OrderCreateRequest
    {
        public string Address { get; set; } = null!;

        /// <summary>Optional cart-wide coupon code typed at checkout.</summary>
        public string? CouponCode { get; set; }
    }
}
