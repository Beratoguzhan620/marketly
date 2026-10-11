using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    /// <summary>
    /// File API ile HTTP üzerinden haberleşen istemcinin sözleşmesi. Application katmanındaki
    /// servisler dosya işlemlerini doğrudan HttpClient veya IFileService ile değil, bu arayüz
    /// üzerinden gerçekleştirir.
    /// </summary>
    public interface IFileApiClient
    {
        Task<ServiceResult<UploadedFileDto>> UploadAsync(FileUploadRequest file, CancellationToken cancellationToken = default);

        Task<ServiceResult> DeleteAsync(string fileKey, CancellationToken cancellationToken = default);
    }
}
