using App.Data.Entities;

namespace App.Eticaret.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public string OrderCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime? StatusUpdatedAt { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = [];
        public decimal Subtotal => Items.Sum(i => i.TotalPrice);
        public decimal Total => Subtotal - DiscountAmount;
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}