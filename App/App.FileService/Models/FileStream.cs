namespace App.FileService.Models
{
    /// <summary>
    /// Bir dosyanın okunabilir Stream'i ile birlikte görüntüleme için gereken ContentType bilgisini taşır.
    /// Stream'i tüketen taraf (controller) dispose etmekle sorumludur.
    /// </summary>
    public class FileStreamResult(Stream content, string contentType, string fileName)
    {
        public Stream Content { get; } = content;
        public string ContentType { get; } = contentType;
        public string FileName { get; } = fileName;
    }
}
