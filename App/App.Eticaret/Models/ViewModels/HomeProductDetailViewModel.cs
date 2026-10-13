using App.Application.Models.Dtos;

namespace App.Eticaret.Models.ViewModels
{
    public class HomeProductDetailViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        /// <summary>List price before discount.</summary>
        public decimal Price { get; set; }

        /// <summary>Price after the active discount.</summary>
        public decimal FinalPrice { get; set; }

        public byte? DiscountRate { get; set; }

        public string Description { get; set; } = null!;

        public byte StockAmount { get; set; }

        public string SellerName { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public int CategoryId { get; set; }

        public string[] ImageUrls { get; set; } = [];

        public ProductReviewViewModel[] Reviews { get; set; } = [];

        public double? AverageStarCount =>
            Reviews.Length == 0 ? null : Reviews.Average(r => r.StarCount);

        public List<ProductListDto> RelatedProducts { get; set; } = [];

        public SaveProductCommentViewModel NewReview { get; set; } = new() { Text = string.Empty, StarCount = 5 };
    }

    public class ProductReviewViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
