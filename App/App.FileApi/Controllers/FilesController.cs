using App.FileApi.Models;
using App.FileService.Interfaces;
using App.FileService.Models;
using App.FileService.Options;
using App.FileService.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace App.FileApi.Controllers
{
    /// <summary>
    /// Dosya yükleme/görüntüleme/silme işlemlerini yönetir. Fiziksel dosya işlemleri
    /// doğrudan burada gerçekleştirilmez; tüm işlemler IFileService üzerinden yapılır.
    /// </summary>
    [ApiController]
    [Route("api/files")]
    [Authorize(AuthenticationSchemes = App.FileApi.Authentication.ApiKeyAuthenticationOptions.SchemeName)]
    public class FilesController(IFileService fileService, IOptions<FileStorageOptions> storageOptions, ILogger<FilesController> logger) : ControllerBase
    {
        private readonly FileStorageOptions _storageOptions = storageOptions.Value;

        [HttpPost]
        [RequestSizeLimit(20_000_000)]
        [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] IFormFile? file)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(ApiErrorResponse.FromFieldError("file", "Yüklenecek dosya boş olamaz."));
            }

            await using var stream = file.OpenReadStream();

            var request = new FileUploadRequest
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length,
                Content = stream
            };

            var result = await fileService.SaveAsync(request);

            if (!result.Success || result.Data is null)
            {
                logger.LogWarning("Dosya yükleme reddedildi. İşlem: Upload, Sonuç: {Message}", result.Message);
                return BadRequest(ApiErrorResponse.FromFieldError("file", result.Message ?? "Dosya yüklenemedi."));
            }

            var response = ToUploadResponse(result.Data);

            return CreatedAtAction(nameof(GetInfo), new { fileKey = response.FileKey }, response);
        }

        [HttpGet("{fileKey}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Download([FromRoute] string fileKey)
        {
            var result = await fileService.ReadAsync(fileKey);

            if (!result.Success || result.Data is null)
            {
                return NotFound(ApiErrorResponse.FromMessage("Dosya bulunamadı."));
            }

            return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
        }

        [HttpGet("{fileKey}/info")]
        [ProducesResponseType(typeof(FileInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInfo([FromRoute] string fileKey)
        {
            var result = await fileService.GetInfoAsync(fileKey);

            if (!result.Success || result.Data is null)
            {
                return NotFound(ApiErrorResponse.FromMessage("Dosya bulunamadı."));
            }

            return Ok(ToInfoResponse(result.Data));
        }

        [HttpDelete("{fileKey}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] string fileKey)
        {
            var result = await fileService.DeleteAsync(fileKey);

            if (!result.Success)
            {
                logger.LogWarning("Dosya silme başarısız. FileKey: {FileKey}, İşlem: Delete", fileKey);

                return result.Type == FileServiceResultType.NotFound
                    ? NotFound(ApiErrorResponse.FromMessage(result.Message ?? "Dosya bulunamadı."))
                    : BadRequest(ApiErrorResponse.FromMessage(result.Message ?? "Dosya silinemedi."));
            }

            return Ok(new { message = result.Message });
        }

        private FileUploadResponse ToUploadResponse(StoredFileMetadata metadata) => new()
        {
            FileKey = metadata.FileKey,
            OriginalFileName = metadata.OriginalFileName,
            ContentType = metadata.ContentType,
            Size = metadata.Size,
            CreatedAt = metadata.CreatedAt,
            Url = BuildPublicUrl(metadata.FileKey)
        };

        private FileInfoResponse ToInfoResponse(StoredFileMetadata metadata) => new()
        {
            FileKey = metadata.FileKey,
            OriginalFileName = metadata.OriginalFileName,
            ContentType = metadata.ContentType,
            Size = metadata.Size,
            CreatedAt = metadata.CreatedAt,
            Url = BuildPublicUrl(metadata.FileKey)
        };

        private string BuildPublicUrl(string fileKey)
        {
            var baseUrl = string.IsNullOrWhiteSpace(_storageOptions.PublicBaseUrl)
                ? $"{Request.Scheme}://{Request.Host}/api/files"
                : _storageOptions.PublicBaseUrl.TrimEnd('/');

            return $"{baseUrl}/{fileKey}";
        }
    }
}
