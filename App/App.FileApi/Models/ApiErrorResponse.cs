namespace App.FileApi.Models
{
    /// <summary>
    /// API'nin hata durumlarında döndürdüğü ortak response modeli. Sunucuya özel fiziksel
    /// klasör yapısı gibi hassas bilgiler bu model üzerinden asla döndürülmemelidir.
    /// </summary>
    public class ApiErrorResponse
    {
        public string Message { get; set; } = null!;

        /// <summary>
        /// Alan bazlı doğrulama hataları (ör. "file" -> ["Dosya boyutu çok büyük."]).
        /// </summary>
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ApiErrorResponse FromMessage(string message) => new() { Message = message };

        public static ApiErrorResponse FromFieldError(string field, string message) => new()
        {
            Message = message,
            Errors = new Dictionary<string, string[]> { [field] = [message] }
        };
    }
}
