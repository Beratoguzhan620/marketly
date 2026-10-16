using App.Data;
using App.Data.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace App.Application.Tests;

/// <summary>
/// A fresh in-memory SQLite database per test, created from the real model including the demo seed
/// (users, products, discounts, coupons). SQLite runs real SQL, so transactions, unique indexes and
/// ExecuteUpdate behave like they do on SQL Server.
/// </summary>
public sealed class TestDatabase : IDisposable
{
    public const int BuyerId = 3;
    public const int OtherBuyerId = 6;

    private readonly SqliteConnection connection;

    public TestDatabase()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        Context = CreateContext();
        Context.Database.EnsureCreated();
    }

    /// <summary>The context the services under test share, like one request scope.</summary>
    public DbContext Context { get; }

    /// <summary>Inside every seeded discount and the KIS2026 coupon window.</summary>
    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 2, 1, 9, 0, 0, TimeSpan.Zero));

    public DataRepository<T> Repo<T>() where T : EntityBase => new(Context);

    /// <summary>A separate context on the same database, to check what was really saved.</summary>
    public DbContext Fresh() => CreateContext();

    private ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options);

    public void Dispose()
    {
        Context.Dispose();
        connection.Dispose();
    }
}
