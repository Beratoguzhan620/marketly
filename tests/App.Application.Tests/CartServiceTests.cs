using App.Application.Results;
using App.Application.Services;
using App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Tests;

public sealed class CartServiceTests : IDisposable
{
    private readonly TestDatabase db = new();
    private readonly CartService service;

    public CartServiceTests()
    {
        service = new CartService(db.Repo<CartItemEntity>(), db.Repo<ProductEntity>());
    }

    public void Dispose() => db.Dispose();

    [Fact]
    public async Task Adding_the_same_product_twice_increases_the_quantity()
    {
        await service.AddProductAsync(TestDatabase.BuyerId, 1);
        await service.AddProductAsync(TestDatabase.BuyerId, 1);

        var item = await db.Fresh().Set<CartItemEntity>().SingleAsync();
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public async Task The_cart_cannot_hold_more_than_the_stock()
    {
        await db.Context.Set<ProductEntity>().Where(p => p.Id == 1).ExecuteUpdateAsync(s => s.SetProperty(p => p.StockAmount, (byte)1));

        var first = await service.AddProductAsync(TestDatabase.BuyerId, 1);
        var second = await service.AddProductAsync(TestDatabase.BuyerId, 1);

        Assert.True(first.Success);
        Assert.Equal(ServiceResultType.ValidationError, second.Type);
    }

    [Fact]
    public async Task A_product_that_is_off_sale_cannot_be_added()
    {
        await db.Context.Set<ProductEntity>().Where(p => p.Id == 1).ExecuteUpdateAsync(s => s.SetProperty(p => p.Enabled, false));

        var result = await service.AddProductAsync(TestDatabase.BuyerId, 1);

        Assert.Equal(ServiceResultType.NotFound, result.Type);
    }

    [Fact]
    public async Task Users_cannot_remove_items_from_someone_elses_cart()
    {
        await service.AddProductAsync(TestDatabase.BuyerId, 1);
        var itemId = (await db.Fresh().Set<CartItemEntity>().SingleAsync()).Id;

        var result = await service.RemoveItemAsync(TestDatabase.OtherBuyerId, itemId);

        Assert.Equal(ServiceResultType.NotFound, result.Type);
        Assert.Equal(1, await db.Fresh().Set<CartItemEntity>().CountAsync());
    }

    [Fact]
    public async Task The_cart_shows_the_discounted_price()
    {
        await service.AddProductAsync(TestDatabase.BuyerId, 2); // Organik Muz, 129,90 with a 10% product discount

        var cart = await service.GetCartAsync(TestDatabase.BuyerId);

        var item = Assert.Single(cart.Items);
        Assert.Equal(129.90m, item.ListPrice);
        Assert.Equal(116.91m, item.Price);
    }
}
