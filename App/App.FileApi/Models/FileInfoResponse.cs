namespace App.FileApi.Models
{
    /// <summary>
    /// Bir dosyanın adı, boyutu, içerik tipi ve oluşturulma bilgilerini taşıyan response modeli.
    /// </summary>
    public class FileInfoResponse
    {
        public string FileKey { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public string Url { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
