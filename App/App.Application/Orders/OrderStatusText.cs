using App.Data.Entities;

namespace App.Application.Orders
{
    /// <summary>Turkish labels and allowed transitions for <see cref="OrderStatus"/>.</summary>
    public static class OrderStatusText
    {
        public static string Label(OrderStatus status) => status switch
        {
            OrderStatus.Preparing => "Hazırlanıyor",
            OrderStatus.Shipped => "Kargoda",
            OrderStatus.Delivered => "Teslim edildi",
            OrderStatus.Cancelled => "İptal edildi",
            _ => status.ToString(),
        };

        /// <summary>
        /// Orders move forward one step at a time; only an order that has not shipped yet can be cancelled.
        /// Delivered and cancelled orders are final.
        /// </summary>
        public static IReadOnlyList<OrderStatus> NextSteps(OrderStatus status) => status switch
        {
            OrderStatus.Preparing => [OrderStatus.Shipped, OrderStatus.Cancelled],
            OrderStatus.Shipped => [OrderStatus.Delivered],
            _ => [],
        };

        public static bool CanMove(OrderStatus from, OrderStatus to) => NextSteps(from).Contains(to);
    }
}
