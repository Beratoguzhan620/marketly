namespace App.Application.Models.Dtos
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public byte? DiscountRate { get; set; }
        public decimal FinalPrice => Pricing.ProductPricing.Apply(Price, DiscountRate);
        public string Description { get; set; } = null!;
        public byte StockAmount { get; set; }
        public string SellerName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int CategoryId { get; set; }
        public string[] ImageUrls { get; set; } = [];
        public List<ProductCommentDto> Comments { get; set; } = [];
    }
}
