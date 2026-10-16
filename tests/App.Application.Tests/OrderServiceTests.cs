using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services;
using App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Tests;

public sealed class OrderServiceTests : IDisposable
{
    private const string Address = "Moda Cad. No: 12 Kadıköy / İstanbul";

    private readonly TestDatabase db = new();
    private readonly OrderService orders;
    private readonly CartService cart;

    public OrderServiceTests()
    {
        orders = new OrderService(
            db.Repo<CartItemEntity>(),
            db.Repo<OrderEntity>(),
            db.Repo<OrderItemEntity>(),
            db.Repo<ProductEntity>(),
            db.Repo<CouponEntity>(),
            db.Clock);
        cart = new CartService(db.Repo<CartItemEntity>(), db.Repo<ProductEntity>());
    }

    public void Dispose() => db.Dispose();

    private async Task FillCartAsync(params int[] productIds)
    {
        foreach (var id in productIds)
            Assert.True((await cart.AddProductAsync(TestDatabase.BuyerId, id)).Success);
    }

    private Task<ServiceResult<App.Application.Models.Dtos.OrderDto>> CheckoutAsync(string? coupon = null) =>
        orders.CreateOrderAsync(TestDatabase.BuyerId, new OrderCreateRequest { Address = Address, CouponCode = coupon });

    [Fact]
    public async Task An_empty_cart_cannot_be_ordered()
    {
        var result = await CheckoutAsync();

        Assert.Equal(ServiceResultType.ValidationError, result.Type);
    }

    [Fact]
    public async Task Checkout_turns_the_cart_into_an_order_and_takes_the_items_from_stock()
    {
        await FillCartAsync(1, 1, 2); // 2 x Muz (64,90) + 1 x Organik Muz (129,90 - 10%)

        var result = await CheckoutAsync();

        Assert.True(result.Success, result.Message);
        Assert.Matches(@"^MK-\d{4}-\d{4}$", result.Data!.OrderCode);
        Assert.Equal(64.90m * 2 + 116.91m, result.Data.TotalPrice);

        var fresh = db.Fresh();
        var order = await fresh.Set<OrderEntity>().Include(o => o.OrderItems).SingleAsync(o => o.OrderCode == result.Data.OrderCode);
        Assert.Equal(OrderStatus.Preparing, order.Status);
        Assert.Equal(Address, order.Address);
        Assert.Equal(116.91m, order.OrderItems.Single(i => i.ProductId == 2).UnitPrice);
        Assert.Empty(fresh.Set<CartItemEntity>());
        Assert.Equal(118, (await fresh.Set<ProductEntity>().SingleAsync(p => p.Id == 1)).StockAmount);
    }

    [Fact]
    public async Task Order_numbers_follow_the_order_id()
    {
        await FillCartAsync(1);
        var first = (await CheckoutAsync()).Data!.OrderCode;
        await FillCartAsync(3);
        var second = (await CheckoutAsync()).Data!.OrderCode;

        // Six orders are seeded, so the next ones are 0007 and 0008.
        Assert.EndsWith("-0007", first);
        Assert.EndsWith("-0008", second);
    }

    [Fact]
    public async Task Nothing_is_ordered_when_a_product_ran_out_after_it_was_added()
    {
        await FillCartAsync(1, 3);
        await db.Context.Set<ProductEntity>().Where(p => p.Id == 3).ExecuteUpdateAsync(s => s.SetProperty(p => p.StockAmount, (byte)0));
        db.Context.ChangeTracker.Clear(); // the cart already loaded the product; read the new stock

        var result = await CheckoutAsync();

        Assert.False(result.Success);
        var fresh = db.Fresh();
        Assert.Equal(6, await fresh.Set<OrderEntity>().CountAsync());
        Assert.Equal(2, await fresh.Set<CartItemEntity>().CountAsync());
        Assert.Equal(120, (await fresh.Set<ProductEntity>().SingleAsync(p => p.Id == 1)).StockAmount);
    }

    [Fact]
    public async Task A_coupon_is_applied_to_the_whole_cart_and_counted_once()
    {
        await FillCartAsync(1, 1, 2); // 246,71 TL, above HOSGELDIN10's 200 TL minimum

        var result = await CheckoutAsync(" hosgeldin10 ");

        Assert.True(result.Success, result.Message);
        Assert.Equal(24.67m, result.Data!.DiscountAmount);
        Assert.Equal(246.71m - 24.67m, result.Data.TotalPrice);

        var fresh = db.Fresh();
        var order = await fresh.Set<OrderEntity>().SingleAsync(o => o.OrderCode == result.Data.OrderCode);
        Assert.Equal("HOSGELDIN10", order.CouponCode);
        Assert.Equal(1, (await fresh.Set<CouponEntity>().SingleAsync(c => c.Code == "HOSGELDIN10")).UsedCount);
    }

    [Fact]
    public async Task A_coupon_below_its_minimum_stops_the_order()
    {
        await FillCartAsync(1); // 64,90 TL

        var result = await CheckoutAsync("HOSGELDIN10");

        Assert.Equal(ServiceResultType.ValidationError, result.Type);
        Assert.Equal(6, await db.Fresh().Set<OrderEntity>().CountAsync());
    }

    [Fact]
    public async Task A_used_up_coupon_is_refused()
    {
        await db.Context.Set<CouponEntity>().Where(c => c.Code == "BUYUKSEPET20")
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.UsedCount, c => c.UsageLimit!.Value));
        await FillCartAsync(30, 30, 30); // well above the 750 TL minimum

        var result = await CheckoutAsync("BUYUKSEPET20");

        Assert.Equal(ServiceResultType.ValidationError, result.Type);
        Assert.Contains("kullanım hakkı", result.Message);
    }

    [Fact]
    public async Task An_expired_coupon_is_refused()
    {
        db.Clock.SetUtcNow(new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.Zero)); // KIS2026 ended in March
        await FillCartAsync(1);

        var result = await CheckoutAsync("KIS2026");

        Assert.Equal(ServiceResultType.ValidationError, result.Type);
    }

    [Fact]
    public async Task Customers_only_see_their_own_orders()
    {
        var result = await orders.GetOrderDetailAsync(TestDatabase.BuyerId, "MK-2026-0002"); // belongs to another customer

        Assert.Equal(ServiceResultType.NotFound, result.Type);
    }
}
