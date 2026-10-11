namespace App.FileService.Options
{
    /// <summary>
    /// Dosyaların fiziksel olarak saklanacağı klasör ve izin verilen dosya kurallarını
    /// configuration (appsettings) üzerinden yönetmeyi sağlar.
    /// </summary>
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";

        /// <summary>
        /// Dosyaların kaydedileceği kök klasör. Göreceli verilirse uygulamanın ContentRoot'una göre çözümlenir.
        /// </summary>
        public string RootPath { get; set; } = "App_Data/uploads";

        /// <summary>
        /// Dosya URL'lerinin üretileceği public taban adres (ör. https://localhost:5001/api/files).
        /// </summary>
        public string PublicBaseUrl { get; set; } = string.Empty;

        public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;

        public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp"];

        public string[] AllowedContentTypes { get; set; } = ["image/jpeg", "image/png", "image/webp"];
    }
}
