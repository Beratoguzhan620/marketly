using App.Application.Models.Dtos;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface ICouponService
    {
        /// <summary>Checks a code against the user's current cart and returns the discount it would give.</summary>
        Task<ServiceResult<CouponQuoteDto>> QuoteForCartAsync(int userId, string? code);
    }
}
