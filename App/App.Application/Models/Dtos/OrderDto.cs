namespace App.Application.Models.Dtos
{
    public class OrderDto
    {
        public string OrderCode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
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
        public List<OrderItemDto> Items { get; set; } = [];
    }
}
