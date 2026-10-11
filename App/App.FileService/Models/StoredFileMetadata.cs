namespace App.FileService.Models
{
    /// <summary>
    /// Kaydedilmiş bir dosyanın API/Application katmanlarına döndürülen metadata bilgisi.
    /// Fiziksel disk yolu bilgisi kasıtlı olarak burada yer almaz.
    /// </summary>
    public class StoredFileMetadata
    {
        public string FileKey { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
