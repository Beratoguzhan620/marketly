using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IContactService
    {
        Task<ServiceResult> SubmitAsync(ContactCreateRequest request);
    }
}
