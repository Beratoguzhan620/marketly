using App.Data;
using App.Data.Entities;
using App.Data.Seeding;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Application.Tests;

/// <summary>The public demo's set-up: the apps on a SQLite file, created with the seed data at start.</summary>
public sealed class SqliteDemoTests : IDisposable
{
    private readonly string file = Path.Combine(Path.GetTempPath(), $"marketly-demo-{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        File.Delete(file);
    }

    [Fact]
    public async Task The_demo_database_is_created_with_the_seed_data_once()
    {
        var services = new ServiceCollection();
        services.AddDataLayer($"Data Source={file}", "Sqlite");
        await using var provider = services.BuildServiceProvider();

        await provider.MigrateDatabaseAsync();
        // The second app starting on the same file finds the schema and changes nothing.
        await provider.MigrateDatabaseAsync();

        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        Assert.Equal(DemoData.Products.Length, await context.Set<ProductEntity>().CountAsync());
        Assert.True(await context.Set<UserEntity>().AnyAsync());
        // Prices can be ordered and summed in SQL on SQLite too (stored as REAL there).
        Assert.True(await context.Set<ProductEntity>().OrderByDescending(p => p.Price).Select(p => p.Price).FirstAsync() > 0);
        Assert.True(await context.Set<ProductEntity>().SumAsync(p => p.Price) > 0);
    }

    [Fact]
    public async Task The_native_sqlite_is_the_patched_build()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "select sqlite_version()";

        var version = Version.Parse((string)(await command.ExecuteScalarAsync())!);

        // CVE-2025-6965 is fixed in SQLite 3.50.2.
        Assert.True(version >= new Version(3, 50, 2), $"SQLite {version} is loaded.");
    }
}
