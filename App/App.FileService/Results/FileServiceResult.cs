namespace App.FileService.Results
{
    /// <summary>
    /// Dosya işlemlerinin (Data içermeyen) sonucunu taşıyan ortak sonuç modeli.
    /// </summary>
    public class FileServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public FileServiceResultType Type { get; set; } = FileServiceResultType.Success;
        public List<string> Errors { get; set; } = [];

        public static FileServiceResult Ok(string? message = null) => new()
        {
            Success = true,
            Message = message,
            Type = FileServiceResultType.Success
        };

        public static FileServiceResult Fail(string message, FileServiceResultType type = FileServiceResultType.Error) => new()
        {
            Success = false,
            Message = message,
            Type = type,
            Errors = [message]
        };

        public static FileServiceResult NotFound(string message = "Dosya bulunamadı.") => Fail(message, FileServiceResultType.NotFound);

        public static FileServiceResult ValidationError(string message) => Fail(message, FileServiceResultType.ValidationError);
    }

    /// <summary>
    /// Dosya işlemlerinin Data döndürdüğü durumlar için kullanılan genel sonuç modeli.
    /// </summary>
    public class FileServiceResult<T> : FileServiceResult
    {
        public T? Data { get; set; }

        public static FileServiceResult<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Data = data,
            Message = message,
            Type = FileServiceResultType.Success
        };

        public static new FileServiceResult<T> Fail(string message, FileServiceResultType type = FileServiceResultType.Error) => new()
        {
            Success = false,
            Message = message,
            Type = type,
            Errors = [message]
        };

        public static new FileServiceResult<T> NotFound(string message = "Dosya bulunamadı.") => Fail(message, FileServiceResultType.NotFound);

        public static new FileServiceResult<T> ValidationError(string message) => Fail(message, FileServiceResultType.ValidationError);
    }

    public enum FileServiceResultType
    {
        Success,
        Error,
        ValidationError,
        NotFound
    }
}
