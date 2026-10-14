using App.Application.Options;
using App.Application.Services;
using App.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace App.Application
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// App.Application projesindeki tüm servisleri DI container'a ekler.
        /// Servisler, kullandıkları DataRepository&lt;TEntity&gt; ve DbContext ile aynı
        /// yaşam döngüsüne (Scoped) sahip olacak şekilde kaydedilir.
        /// </summary>
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.TryAddSingleton(TimeProvider.System);
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductCommentService, ProductCommentService>();
            services.AddScoped<IContactService, ContactService>();

            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

            services.AddFileApiClient(configuration);
        }

        /// <summary>
        /// IFileApiClient'ı typed HttpClient olarak, File API base address/timeout/ApiKey
        /// bilgilerini appsettings (ve gizli bilgiler için User Secrets/environment variable) üzerinden okuyarak kaydeder.
        /// </summary>
        private static void AddFileApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(FileApiOptions.SectionName).Get<FileApiOptions>() ?? new FileApiOptions();

            services.AddHttpClient<IFileApiClient, FileApiClient>(client =>
            {
                client.BaseAddress = new Uri(options.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

                if (!string.IsNullOrWhiteSpace(options.ApiKey))
                {
                    client.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
                }
            });
        }
    }
}
