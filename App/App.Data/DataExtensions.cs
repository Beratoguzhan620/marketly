using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Data
{
    public static class DataExtensions
    {
        /// <param name="provider">
        /// "SqlServer" (the default) or "Sqlite". SQLite is for the public demo: one small container with the
        /// database in a file that starts again from the seed data on every restart.
        /// </param>
        public static void AddDataLayer(this IServiceCollection services, string connectionString, string? provider = null)
        {
            var sqlite = string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase);
            services.AddDbContext<DbContext, ApplicationDbContext>(options =>
            {
                if (sqlite)
                    options.UseSqlite(connectionString);
                else
                    options.UseSqlServer(connectionString);
            });

            services.AddScoped(typeof(DataRepository<>));
        }

        /// <summary>
        /// Applies pending EF Core migrations (including seed data). Safe to call from several
        /// apps at start-up: EF Core takes a database lock while migrating. On SQLite (the demo) the schema and seed
        /// data are created from the model instead, as the migrations are written for SQL Server.
        /// </summary>
        public static async Task MigrateDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
        {
            await using var scope = services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContext>();
            if (context.Database.IsSqlite())
                await context.Database.EnsureCreatedAsync(cancellationToken);
            else
                await context.Database.MigrateAsync(cancellationToken);
        }
    }
}
