using System.Globalization;

namespace App.Admin.Infrastructure
{
    public static class Display
    {
        public static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

        public static string Money(decimal amount) => amount.ToString("C2", Turkish);

        public static string Date(DateTime utc) => utc.ToLocalTime().ToString("d MMM yyyy", Turkish);

        public static string DateTime(DateTime utc) => utc.ToLocalTime().ToString("d MMM yyyy, HH:mm", Turkish);

        public static string OrderStatus(App.Data.Entities.OrderStatus status) => App.Application.Orders.OrderStatusText.Label(status);

        public static string OrderStatusBadge(App.Data.Entities.OrderStatus status) => status switch
        {
            App.Data.Entities.OrderStatus.Delivered => "badge badge-success",
            App.Data.Entities.OrderStatus.Shipped => "badge badge-accent",
            App.Data.Entities.OrderStatus.Cancelled => "badge badge-danger",
            _ => "badge badge-warning",
        };

        public static string MonthShort(int year, int month) => new DateTime(year, month, 1).ToString("MMM", Turkish);

        public static string RoleLabel(string role) => role.ToLowerInvariant() switch
        {
            "admin" => "Yönetici",
            "seller" => "Satıcı",
            "buyer" => "Alıcı",
            _ => role,
        };
    }
}
