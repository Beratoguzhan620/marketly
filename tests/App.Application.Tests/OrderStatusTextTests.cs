using App.Application.Orders;
using App.Data.Entities;

namespace App.Application.Tests;

public class OrderStatusTextTests
{
    [Theory]
    [InlineData(OrderStatus.Preparing, OrderStatus.Shipped, true)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Cancelled, true)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Delivered, true)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Delivered, false)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Cancelled, false)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Preparing, false)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Cancelled, false)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Preparing, false)]
    public void Orders_move_forward_one_step_and_can_be_cancelled_only_before_shipping(OrderStatus from, OrderStatus to, bool allowed) =>
        Assert.Equal(allowed, OrderStatusText.CanMove(from, to));

    [Fact]
    public void Every_status_has_a_turkish_label()
    {
        foreach (var status in Enum.GetValues<OrderStatus>())
            Assert.DoesNotContain(status.ToString(), OrderStatusText.Label(status));
    }
}
