using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Data
{
    public static class DataExtensions
    {
        public static void AddDataLayer(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<DbContext, ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped(typeof(DataRepository<>));
        }

        /// <summary>
        /// Applies pending EF Core migrations (including seed data). Safe to call from several
        /// apps at start-up: EF Core takes a database lock while migrating.
        /// </summary>
        public static async Task MigrateDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
        {
            await using var scope = services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContext>();
            await context.Database.MigrateAsync(cancellationToken);
        }
    }
}
