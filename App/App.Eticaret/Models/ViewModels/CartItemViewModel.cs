namespace App.Eticaret.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? ProductImage { get; set; }

        public byte Quantity { get; set; }

        /// <summary>Unit price after discount.</summary>
        public decimal Price { get; set; }

        /// <summary>Unit price before discount.</summary>
        public decimal ListPrice { get; set; }

        public byte? DiscountRate { get; set; }

        public byte StockAmount { get; set; }

        public decimal TotalPrice => Quantity * Price;

        public decimal ListTotalPrice => Quantity * ListPrice;
    }
}
