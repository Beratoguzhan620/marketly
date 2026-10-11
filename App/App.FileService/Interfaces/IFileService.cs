using App.FileService.Models;
using App.FileService.Results;

namespace App.FileService.Interfaces
{
    /// <summary>
    /// Dosya yükleme, silme, bilgi görüntüleme ve akış okuma işlemlerini dışarıya açan servis sözleşmesi.
    /// Implementasyon local disk üzerinde çalışır; ileride Azure Blob/S3 gibi farklı bir depolama
    /// sağlayıcısına bu arayüz korunarak geçilebilir.
    /// </summary>
    public interface IFileService
    {
        Task<FileServiceResult<StoredFileMetadata>> SaveAsync(FileUploadRequest request);

        Task<FileServiceResult<FileStreamResult>> ReadAsync(string fileKey);

        Task<FileServiceResult<StoredFileMetadata>> GetInfoAsync(string fileKey);

        Task<FileServiceResult> DeleteAsync(string fileKey);
    }
}
