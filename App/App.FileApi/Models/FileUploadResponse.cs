namespace App.FileApi.Models
{
    /// <summary>
    /// Dosya yükleme sonucunda döndürülen ve sunucuya özel fiziksel yol bilgisi içermeyen response modeli.
    /// </summary>
    public class FileUploadResponse
    {
        public string FileKey { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public string Url { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
