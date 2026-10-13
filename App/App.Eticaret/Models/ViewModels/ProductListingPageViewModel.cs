using App.Application.Models.Dtos;
using App.Application.Models.Requests;

namespace App.Eticaret.Models.ViewModels
{
    public class ProductListingPageViewModel
    {
        public ProductListingQuery Query { get; set; } = new();
        public PagedResult<ProductListDto> Result { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = [];

        public CategoryDto? SelectedCategory => Categories.FirstOrDefault(c => c.Id == Query.CategoryId);

        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(Query.Search)
            || Query.CategoryId is not null
            || Query.MinPrice is not null
            || Query.MaxPrice is not null
            || Query.OnlyDiscounted;

        public static readonly (ProductSort Value, string Text)[] SortOptions =
        [
            (ProductSort.Newest, "En yeniler"),
            (ProductSort.PriceAsc, "Fiyat: düşükten yükseğe"),
            (ProductSort.PriceDesc, "Fiyat: yüksekten düşüğe"),
            (ProductSort.TopRated, "En çok beğenilenler"),
            (ProductSort.Name, "Ada göre (A-Z)"),
        ];

        /// <summary>Route values for the current filters, with optional overrides.</summary>
        public Dictionary<string, string?> RouteValues(
            int? page = null,
            bool clearSearch = false,
            bool clearCategory = false,
            bool clearPrice = false,
            bool clearDiscount = false,
            int? category = null)
        {
            var values = new Dictionary<string, string?>
            {
                ["search"] = clearSearch ? null : Query.Search,
                ["category"] = clearCategory ? null : (category ?? Query.CategoryId)?.ToString(),
                ["min"] = clearPrice ? null : Query.MinPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["max"] = clearPrice ? null : Query.MaxPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["discounted"] = clearDiscount || !Query.OnlyDiscounted ? null : "true",
                ["sort"] = Query.Sort == ProductSort.Newest ? null : Query.Sort.ToString(),
                ["page"] = page is > 1 ? page.ToString() : null,
            };

            return values.Where(kv => !string.IsNullOrEmpty(kv.Value)).ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
