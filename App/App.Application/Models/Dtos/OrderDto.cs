using App.Data.Entities;

namespace App.Application.Models.Dtos
{
    public class OrderDto
    {
        public string OrderCode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice => Subtotal - DiscountAmount;
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public class OrderDetailDto
    {
        public string OrderCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public DateTime? StatusUpdatedAt { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = [];
        public decimal Subtotal => Items.Sum(i => i.TotalPrice);
        public decimal Total => Subtotal - DiscountAmount;
    }
}
