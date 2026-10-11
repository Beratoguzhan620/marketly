using App.FileService.Interfaces;
using App.FileService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.FileService
{
    public static class FileServiceExtensions
    {
        /// <summary>
        /// App.FileService içerisindeki servisleri ve FileStorageOptions ayarlarını DI container'a ekler.
        /// </summary>
        public static void AddFileServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
            services.AddSingleton<IFileService, Services.FileService>();
        }
    }
}
