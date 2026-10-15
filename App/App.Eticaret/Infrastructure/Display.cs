using System.Globalization;

namespace App.Eticaret.Infrastructure
{
    /// <summary>
    /// Formatting helpers for the storefront. Model binding keeps the invariant culture
    /// (prices are posted as "64.90"), so Turkish formatting is applied only for display.
    /// </summary>
    public static class Display
    {
        public static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

        public static string Money(decimal amount) => amount.ToString("C2", Turkish);

        public static string Date(DateTime value) => value.ToLocalTime().ToString("d MMMM yyyy", Turkish);

        public static string DateTime(DateTime value) => value.ToLocalTime().ToString("d MMMM yyyy, HH:mm", Turkish);

        public static string OrderStatus(App.Data.Entities.OrderStatus status) => App.Application.Orders.OrderStatusText.Label(status);

        public static string OrderStatusBadge(App.Data.Entities.OrderStatus status) => status switch
        {
            App.Data.Entities.OrderStatus.Delivered => "badge badge-success",
            App.Data.Entities.OrderStatus.Shipped => "badge badge-accent",
            App.Data.Entities.OrderStatus.Cancelled => "badge badge-danger",
            _ => "badge badge-warning",
        };

        public static string Rating(double value) => value.ToString("0.0", Turkish);

        private static readonly (string Keyword, string Icon)[] CategoryIcons =
        [
            ("et", "bi-fire"),
            ("tavuk", "bi-fire"),
            ("sebze", "bi-flower2"),
            ("meyve", "bi-basket2"),
            ("kuruyemiş", "bi-bag"),
            ("deniz", "bi-water"),
            ("süt", "bi-cup-straw"),
            ("kahvaltı", "bi-egg"),
            ("yemek", "bi-egg-fried"),
            ("tahıl", "bi-grid-3x3-gap"),
            ("bakliyat", "bi-grid-3x3-gap"),
            ("içecek", "bi-cup-hot"),
        ];

        /// <summary>Bootstrap Icons class for a category; falls back to a neutral box.</summary>
        public static string CategoryIcon(string? categoryName, string? iconCssClass = null)
        {
            if (!string.IsNullOrWhiteSpace(iconCssClass) && iconCssClass.StartsWith("bi-", StringComparison.Ordinal))
            {
                return iconCssClass;
            }

            var name = (categoryName ?? string.Empty).ToLower(Turkish);
            var words = name.Split([' ', '&', ','], StringSplitOptions.RemoveEmptyEntries);

            foreach (var (keyword, icon) in CategoryIcons)
            {
                if (words.Any(w => w.StartsWith(keyword, StringComparison.Ordinal)))
                {
                    return icon;
                }
            }

            return "bi-box-seam";
        }

        /// <summary>One of nine muted tints (tint-1 … tint-9), stable per category.</summary>
        public static string CategoryTint(int categoryId) => $"tint-{((Math.Abs(categoryId) - 1) % 9 + 9) % 9 + 1}";
    }
}
