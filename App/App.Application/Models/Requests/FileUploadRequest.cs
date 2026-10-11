namespace App.Application.Models.Requests
{
    /// <summary>
    /// Controller katmanından gelen IFormFile nesnesinin Application katmanına Stream tabanlı
    /// olarak aktarılmasını sağlayan bağımsız model. Application katmanı MVC'ye ait IFormFile
    /// tipini bilmez.
    /// </summary>
    public class FileUploadRequest
    {
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Length { get; set; }
        public Stream Content { get; set; } = null!;
    }
}
