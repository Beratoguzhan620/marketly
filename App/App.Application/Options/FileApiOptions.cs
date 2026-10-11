namespace App.Application.Options
{
    /// <summary>
    /// File API ile haberleşecek FileApiClient'ın base address, timeout ve authentication
    /// bilgilerini appsettings.json (ve gizli bilgiler için User Secrets/environment variable) üzerinden okur.
    /// </summary>
    public class FileApiOptions
    {
        public const string SectionName = "FileApi";

        public string BaseAddress { get; set; } = null!;

        public int TimeoutSeconds { get; set; } = 30;

        public string ApiKey { get; set; } = string.Empty;
    }
}
