using System.Net;
using System.Text.Json;
using App.FileApi.Models;

namespace App.FileApi.Middleware
{
    /// <summary>
    /// Beklenmeyen hataları merkezi olarak yakalayıp kullanıcıya sunucuya özel bilgi
    /// (fiziksel klasör yapısı, stack trace vb.) içermeyen, tutarlı bir ApiErrorResponse döner.
    /// </summary>
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "İşlenmeyen bir hata oluştu. Path: {Path}", context.Request.Path);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = ApiErrorResponse.FromMessage("Beklenmeyen bir sunucu hatası oluştu.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
