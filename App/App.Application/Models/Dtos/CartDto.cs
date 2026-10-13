namespace App.Application.Models.Dtos
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? ProductImage { get; set; }
        public byte Quantity { get; set; }
        /// <summary>Unit price after the product discount.</summary>
        public decimal Price { get; set; }
        /// <summary>Unit price before any discount.</summary>
        public decimal ListPrice { get; set; }
        public byte? DiscountRate { get; set; }
        public byte StockAmount { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }

    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = [];
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    }
}
