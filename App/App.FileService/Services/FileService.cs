using System.Text.Json;
using App.FileService.Interfaces;
using App.FileService.Models;
using App.FileService.Options;
using App.FileService.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.FileService.Services
{
    public class FileService : IFileService
    {
        private const string MetadataExtension = ".meta.json";

        private readonly FileStorageOptions _options;
        private readonly ILogger<FileService> _logger;
        private readonly string _rootPath;

        public FileService(IOptions<FileStorageOptions> options, ILogger<FileService> logger)
        {
            _options = options.Value;
            _logger = logger;

            _rootPath = Path.IsPathRooted(_options.RootPath)
                ? _options.RootPath
                : Path.Combine(AppContext.BaseDirectory, _options.RootPath);

            Directory.CreateDirectory(_rootPath);
        }

        public async Task<FileServiceResult<StoredFileMetadata>> SaveAsync(FileUploadRequest request)
        {
            if (request.Length <= 0 || request.Content is null)
            {
                return FileServiceResult<StoredFileMetadata>.ValidationError("Boş veya okunamayan dosya yüklenemez.");
            }

            if (request.Length > _options.MaxFileSizeBytes)
            {
                return FileServiceResult<StoredFileMetadata>.ValidationError(
                    $"Dosya boyutu izin verilen en fazla {_options.MaxFileSizeBytes / (1024 * 1024)} MB sınırını aşıyor.");
            }

            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !_options.AllowedExtensions.Contains(extension))
            {
                return FileServiceResult<StoredFileMetadata>.ValidationError("Bu dosya uzantısına izin verilmiyor.");
            }

            if (string.IsNullOrWhiteSpace(request.ContentType) || !_options.AllowedContentTypes.Contains(request.ContentType.ToLowerInvariant()))
            {
                return FileServiceResult<StoredFileMetadata>.ValidationError("Bu dosya içerik tipine (ContentType) izin verilmiyor.");
            }

            var signatureValid = await MatchesKnownImageSignatureAsync(request.Content);
            if (!signatureValid)
            {
                return FileServiceResult<StoredFileMetadata>.ValidationError("Dosya içeriği geçerli bir görsel formatına uymuyor.");
            }

            var fileKey = $"{Guid.NewGuid():N}{extension}";
            var physicalPath = ResolvePhysicalPath(fileKey);

            if (physicalPath is null)
            {
                return FileServiceResult<StoredFileMetadata>.ValidationError("Geçersiz dosya anahtarı.");
            }

            var metadata = new StoredFileMetadata
            {
                FileKey = fileKey,
                OriginalFileName = Path.GetFileName(request.FileName),
                ContentType = request.ContentType,
                Size = request.Length,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await using (var destination = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                {
                    request.Content.Seek(0, SeekOrigin.Begin);
                    await request.Content.CopyToAsync(destination);
                }

                await WriteMetadataAsync(fileKey, metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya kaydedilirken beklenmeyen bir hata oluştu. FileKey: {FileKey}", fileKey);
                TryDeletePhysical(fileKey);
                return FileServiceResult<StoredFileMetadata>.Fail("Dosya kaydedilirken beklenmeyen bir hata oluştu.");
            }

            _logger.LogInformation("Dosya kaydedildi. FileKey: {FileKey}, İşlem: Save, Tarih: {Date}", fileKey, metadata.CreatedAt);

            return FileServiceResult<StoredFileMetadata>.Ok(metadata, "Dosya başarıyla kaydedildi.");
        }

        public async Task<FileServiceResult<FileStreamResult>> ReadAsync(string fileKey)
        {
            var physicalPath = ResolvePhysicalPath(fileKey);

            if (physicalPath is null || !File.Exists(physicalPath))
            {
                return FileServiceResult<FileStreamResult>.NotFound();
            }

            var metadataResult = await ReadMetadataAsync(fileKey);
            var contentType = metadataResult?.ContentType ?? "application/octet-stream";
            var fileName = metadataResult?.OriginalFileName ?? fileKey;

            var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);

            return FileServiceResult<FileStreamResult>.Ok(new FileStreamResult(stream, contentType, fileName));
        }

        public async Task<FileServiceResult<StoredFileMetadata>> GetInfoAsync(string fileKey)
        {
            var physicalPath = ResolvePhysicalPath(fileKey);

            if (physicalPath is null || !File.Exists(physicalPath))
            {
                return FileServiceResult<StoredFileMetadata>.NotFound();
            }

            var metadata = await ReadMetadataAsync(fileKey);

            if (metadata is null)
            {
                return FileServiceResult<StoredFileMetadata>.NotFound();
            }

            return FileServiceResult<StoredFileMetadata>.Ok(metadata);
        }

        public Task<FileServiceResult> DeleteAsync(string fileKey)
        {
            var physicalPath = ResolvePhysicalPath(fileKey);

            if (physicalPath is null || !File.Exists(physicalPath))
            {
                return Task.FromResult((FileServiceResult)FileServiceResult.NotFound());
            }

            try
            {
                File.Delete(physicalPath);

                var metadataPath = physicalPath + MetadataExtension;
                if (File.Exists(metadataPath))
                {
                    File.Delete(metadataPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya silinirken beklenmeyen bir hata oluştu. FileKey: {FileKey}", fileKey);
                return Task.FromResult((FileServiceResult)FileServiceResult.Fail("Dosya silinirken beklenmeyen bir hata oluştu."));
            }

            _logger.LogInformation("Dosya silindi. FileKey: {FileKey}, İşlem: Delete, Tarih: {Date}", fileKey, DateTime.UtcNow);

            return Task.FromResult((FileServiceResult)FileServiceResult.Ok("Dosya başarıyla silindi."));
        }

        /// <summary>
        /// Kullanıcıdan gelen fileKey'in yalnızca sistem tarafından üretilmiş bir dosya adı olduğunu
        /// doğrular ve path traversal riskine karşı sonuç yolunun kök klasörün dışına çıkmadığını kontrol eder.
        /// </summary>
        private string? ResolvePhysicalPath(string fileKey)
        {
            if (string.IsNullOrWhiteSpace(fileKey))
            {
                return null;
            }

            var safeFileName = Path.GetFileName(fileKey);
            if (safeFileName != fileKey)
            {
                return null;
            }

            var combined = Path.GetFullPath(Path.Combine(_rootPath, safeFileName));
            var rootFullPath = Path.GetFullPath(_rootPath);

            if (!combined.StartsWith(rootFullPath, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return combined;
        }

        private async Task WriteMetadataAsync(string fileKey, StoredFileMetadata metadata)
        {
            var physicalPath = ResolvePhysicalPath(fileKey)!;
            var metadataPath = physicalPath + MetadataExtension;

            await using var stream = new FileStream(metadataPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await JsonSerializer.SerializeAsync(stream, metadata);
        }

        private async Task<StoredFileMetadata?> ReadMetadataAsync(string fileKey)
        {
            var physicalPath = ResolvePhysicalPath(fileKey);
            if (physicalPath is null)
            {
                return null;
            }

            var metadataPath = physicalPath + MetadataExtension;
            if (!File.Exists(metadataPath))
            {
                return null;
            }

            await using var stream = new FileStream(metadataPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            return await JsonSerializer.DeserializeAsync<StoredFileMetadata>(stream);
        }

        private void TryDeletePhysical(string fileKey)
        {
            try
            {
                var physicalPath = ResolvePhysicalPath(fileKey);
                if (physicalPath is not null && File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch
            {
                // Telafi silme işlemi sırasında oluşan hata orijinal hatayı gölgelememelidir.
            }
        }

        /// <summary>
        /// Yalnızca ContentType/uzantı beyanına güvenmemek için dosyanın ilk baytlarını
        /// bilinen görsel format imzalarıyla (magic number) karşılaştırır.
        /// </summary>
        private static async Task<bool> MatchesKnownImageSignatureAsync(Stream content)
        {
            var buffer = new byte[12];
            content.Seek(0, SeekOrigin.Begin);
            var bytesRead = await content.ReadAsync(buffer.AsMemory(0, buffer.Length));
            content.Seek(0, SeekOrigin.Begin);

            if (bytesRead < 4)
            {
                return false;
            }

            // JPEG: FF D8 FF
            if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            {
                return true;
            }

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (bytesRead >= 8 && buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
            {
                return true;
            }

            // WEBP: "RIFF"....."WEBP"
            if (bytesRead >= 12 &&
                buffer[0] == 'R' && buffer[1] == 'I' && buffer[2] == 'F' && buffer[3] == 'F' &&
                buffer[8] == 'W' && buffer[9] == 'E' && buffer[10] == 'B' && buffer[11] == 'P')
            {
                return true;
            }

            return false;
        }
    }
}
