namespace App.Application.Models.Dtos
{
    /// <summary>
    /// File API'ye başarıyla yüklenmiş bir dosyanın Application katmanına dönen bilgisi.
    /// </summary>
    public class UploadedFileDto
    {
        public string FileKey { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
