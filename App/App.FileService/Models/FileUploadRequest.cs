namespace App.FileService.Models
{
    /// <summary>
    /// MVC/IFormFile tipine bağımlı olmadan dosya yükleme işlemini temsil eden bağımsız model.
    /// Çağıran katman (Application/API) dosya içeriğini Stream olarak sağlar.
    /// </summary>
    public class FileUploadRequest
    {
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Length { get; set; }
        public Stream Content { get; set; } = null!;
    }
}
