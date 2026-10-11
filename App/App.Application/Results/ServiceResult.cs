namespace App.Application.Results
{
    /// <summary>
    /// Servis metotlarının işlem sonucunu (Data içermeyen) taşıyan ortak sonuç modeli.
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ServiceResultType Type { get; set; } = ServiceResultType.Success;
        public List<string> Errors { get; set; } = [];

        public static ServiceResult Ok(string? message = null) => new()
        {
            Success = true,
            Message = message,
            Type = ServiceResultType.Success
        };

        public static ServiceResult Fail(string message, ServiceResultType type = ServiceResultType.Error) => new()
        {
            Success = false,
            Message = message,
            Type = type,
            Errors = [message]
        };

        public static ServiceResult NotFound(string message = "Kayıt bulunamadı.") => Fail(message, ServiceResultType.NotFound);

        public static ServiceResult Forbidden(string message = "Bu işlemi gerçekleştirmeye yetkiniz yok.") => Fail(message, ServiceResultType.Forbidden);

        public static ServiceResult ValidationError(string message) => Fail(message, ServiceResultType.ValidationError);
    }

    /// <summary>
    /// Servis metotlarının Data döndürdüğü durumlar için kullanılan genel sonuç modeli.
    /// </summary>
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Data = data,
            Message = message,
            Type = ServiceResultType.Success
        };

        public static new ServiceResult<T> Fail(string message, ServiceResultType type = ServiceResultType.Error) => new()
        {
            Success = false,
            Message = message,
            Type = type,
            Errors = [message]
        };

        public static new ServiceResult<T> NotFound(string message = "Kayıt bulunamadı.") => Fail(message, ServiceResultType.NotFound);

        public static new ServiceResult<T> Forbidden(string message = "Bu işlemi gerçekleştirmeye yetkiniz yok.") => Fail(message, ServiceResultType.Forbidden);

        public static new ServiceResult<T> ValidationError(string message) => Fail(message, ServiceResultType.ValidationError);
    }

    public enum ServiceResultType
    {
        Success,
        Error,
        ValidationError,
        NotFound,
        Forbidden
    }
}
