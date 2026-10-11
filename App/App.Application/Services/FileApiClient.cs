using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace App.Application.Services
{
    /// <summary>
    /// File API'ye HttpClient (typed client, IHttpClientFactory üzerinden yönetilir) ile istek atar.
    /// Ağ/timeout/başarısız response durumlarını yakalayıp Application katmanına anlaşılır ServiceResult döner.
    /// </summary>
    public class FileApiClient(HttpClient httpClient, ILogger<FileApiClient> logger) : IFileApiClient
    {
        public async Task<ServiceResult<UploadedFileDto>> UploadAsync(FileUploadRequest file, CancellationToken cancellationToken = default)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(file.Content);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                content.Add(streamContent, "file", file.FileName);

                // Dosya yükleme idempotent bir işlem olmadığından (tekrar denemek mükerrer
                // dosya oluşturabileceğinden) burada otomatik retry uygulanmaz.
                using var response = await httpClient.PostAsync("api/files", content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var uploaded = await response.Content.ReadFromJsonAsync<FileApiUploadPayload>(cancellationToken: cancellationToken);

                    if (uploaded is null)
                    {
                        return ServiceResult<UploadedFileDto>.Fail("File API geçersiz bir yanıt döndürdü.");
                    }

                    return ServiceResult<UploadedFileDto>.Ok(new UploadedFileDto
                    {
                        FileKey = uploaded.FileKey,
                        Url = uploaded.Url,
                        OriginalFileName = uploaded.OriginalFileName,
                        ContentType = uploaded.ContentType,
                        Size = uploaded.Size,
                        CreatedAt = uploaded.CreatedAt
                    });
                }

                var errorMessage = await ExtractErrorMessageAsync(response, cancellationToken);
                logger.LogWarning("Dosya yükleme başarısız. Durum kodu: {StatusCode}, Dosya: {FileName}", response.StatusCode, file.FileName);

                return response.StatusCode == HttpStatusCode.BadRequest
                    ? ServiceResult<UploadedFileDto>.ValidationError(errorMessage)
                    : ServiceResult<UploadedFileDto>.Fail(errorMessage);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex, "File API'ye dosya yükleme isteği zaman aşımına uğradı. Dosya: {FileName}", file.FileName);
                return ServiceResult<UploadedFileDto>.Fail("Dosya yükleme servisine ulaşılamadı (zaman aşımı). Lütfen daha sonra tekrar deneyin.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "File API'ye bağlanırken bir hata oluştu. Dosya: {FileName}", file.FileName);
                return ServiceResult<UploadedFileDto>.Fail("Dosya yükleme servisine şu anda ulaşılamıyor. Lütfen daha sonra tekrar deneyin.");
            }
        }

        public async Task<ServiceResult> DeleteAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            const int maxAttempts = 3;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    // Silme işlemi idempotent kabul edilir (dosya zaten silinmişse API 404 döner),
                    // bu yüzden geçici ağ hatalarında kontrollü şekilde tekrar denenir.
                    using var response = await httpClient.DeleteAsync($"api/files/{fileKey}", cancellationToken);

                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return ServiceResult.Ok("Dosya silindi.");
                    }

                    var errorMessage = await ExtractErrorMessageAsync(response, cancellationToken);

                    if (!IsTransient(response.StatusCode) || attempt == maxAttempts)
                    {
                        logger.LogWarning("Dosya silme başarısız. FileKey: {FileKey}, Durum kodu: {StatusCode}", fileKey, response.StatusCode);
                        return ServiceResult.Fail(errorMessage);
                    }
                }
                catch (Exception ex) when (ex is OperationCanceledException or HttpRequestException)
                {
                    if (attempt == maxAttempts)
                    {
                        logger.LogError(ex, "Dosya silme isteği tekrar denemelere rağmen başarısız oldu. FileKey: {FileKey}", fileKey);
                        return ServiceResult.Fail("Dosya silme servisine şu anda ulaşılamıyor.");
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), cancellationToken);
            }

            return ServiceResult.Fail("Dosya silme servisine şu anda ulaşılamıyor.");
        }

        private static bool IsTransient(HttpStatusCode statusCode) =>
            statusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests or >= HttpStatusCode.InternalServerError;

        private static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            try
            {
                var payload = await response.Content.ReadFromJsonAsync<FileApiErrorPayload>(cancellationToken: cancellationToken);
                return payload?.Message ?? "Dosya işlemi sırasında bir hata oluştu.";
            }
            catch
            {
                return "Dosya işlemi sırasında bir hata oluştu.";
            }
        }

        private class FileApiUploadPayload
        {
            public string FileKey { get; set; } = null!;
            public string OriginalFileName { get; set; } = null!;
            public string ContentType { get; set; } = null!;
            public long Size { get; set; }
            public string Url { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
        }

        private class FileApiErrorPayload
        {
            public string? Message { get; set; }
        }
    }
}
