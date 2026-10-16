using App.Application.Results;
using App.Application.Services;
using App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Tests;

public sealed class AdminOrderStatusTests : IDisposable
{
    // Seeded orders: 4 is shipped, 5 is being prepared, 1 is delivered.
    private const int ShippedOrder = 4;
    private const int PreparingOrder = 5;
    private const int DeliveredOrder = 1;

    private readonly TestDatabase db = new();
    private readonly AdminService service;

    public AdminOrderStatusTests()
    {
        service = new AdminService(
            db.Repo<OrderEntity>(),
            db.Repo<OrderItemEntity>(),
            db.Repo<UserEntity>(),
            db.Repo<ProductEntity>(),
            db.Repo<ProductCommentEntity>(),
            db.Repo<BlogCommentEntity>(),
            db.Repo<CouponEntity>(),
            db.Clock);
    }

    public void Dispose() => db.Dispose();

    private async Task<OrderEntity> OrderAsync(int id) => await db.Fresh().Set<OrderEntity>().SingleAsync(o => o.Id == id);

    [Fact]
    public async Task Shipping_an_order_records_when_it_happened()
    {
        var result = await service.UpdateOrderStatusAsync(PreparingOrder, OrderStatus.Shipped);

        Assert.True(result.Success, result.Message);
        var order = await OrderAsync(PreparingOrder);
        Assert.Equal(OrderStatus.Shipped, order.Status);
        Assert.Equal(db.Clock.GetUtcNow().UtcDateTime, order.StatusUpdatedAt);
    }

    [Theory]
    [InlineData(ShippedOrder, OrderStatus.Cancelled)]
    [InlineData(DeliveredOrder, OrderStatus.Preparing)]
    [InlineData(PreparingOrder, OrderStatus.Delivered)]
    public async Task Invalid_moves_are_refused(int orderId, OrderStatus target)
    {
        var before = (await OrderAsync(orderId)).Status;

        var result = await service.UpdateOrderStatusAsync(orderId, target);

        Assert.Equal(ServiceResultType.ValidationError, result.Type);
        Assert.Equal(before, (await OrderAsync(orderId)).Status);
    }

    [Fact]
    public async Task Cancelling_puts_the_items_back_into_stock()
    {
        // Order 5: 1 x Siyah Üzüm (5), 3 x Mango (7), 1 x Renkli Biber (19).
        var stockBefore = await db.Fresh().Set<ProductEntity>().Where(p => p.Id == 5 || p.Id == 7 || p.Id == 19)
            .ToDictionaryAsync(p => p.Id, p => p.StockAmount);

        var result = await service.UpdateOrderStatusAsync(PreparingOrder, OrderStatus.Cancelled);

        Assert.True(result.Success, result.Message);
        var stockAfter = await db.Fresh().Set<ProductEntity>().Where(p => p.Id == 5 || p.Id == 7 || p.Id == 19)
            .ToDictionaryAsync(p => p.Id, p => p.StockAmount);
        Assert.Equal(stockBefore[5] + 1, stockAfter[5]);
        Assert.Equal(stockBefore[7] + 3, stockAfter[7]);
        Assert.Equal(stockBefore[19] + 1, stockAfter[19]);
    }

    [Fact]
    public async Task An_unknown_order_is_not_found()
    {
        var result = await service.UpdateOrderStatusAsync(999, OrderStatus.Shipped);

        Assert.Equal(ServiceResultType.NotFound, result.Type);
    }
}
