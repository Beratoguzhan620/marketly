namespace App.Application.Models.Dtos
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string SellerName { get; set; } = null!;

        /// <summary>Price before discount.</summary>
        public decimal Price { get; set; }

        /// <summary>Price the customer pays today.</summary>
        public decimal FinalPrice { get; set; }

        public byte? DiscountPercentage { get; set; }
        public byte StockAmount { get; set; }
        public string? ImageUrl { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
