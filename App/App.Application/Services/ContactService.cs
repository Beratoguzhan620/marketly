using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;

namespace App.Application.Services
{
    public class ContactService(DataRepository<ContactFormEntity> contactFormRepo) : IContactService
    {
        public async Task<ServiceResult> SubmitAsync(ContactCreateRequest request)
        {
            var entity = new ContactFormEntity
            {
                Name = request.Name,
                Email = request.Email,
                Message = request.Message,
                SeenAt = null
            };

            await contactFormRepo.AddAsync(entity);

            return ServiceResult.Ok("Your message has been sent successfully.");
        }
    }
}
