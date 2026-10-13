namespace App.Application.Models.Requests
{
    public enum ProductSort
    {
        Newest,
        PriceAsc,
        PriceDesc,
        Name,
        TopRated,
    }

    public class ProductListingQuery
    {
        public const int DefaultPageSize = 12;
        public const int MaxPageSize = 48;

        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool OnlyDiscounted { get; set; }
        public int? ExcludeProductId { get; set; }
        public ProductSort Sort { get; set; } = ProductSort.Newest;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
    }
}
