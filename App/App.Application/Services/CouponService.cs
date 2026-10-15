using App.Application.Models.Dtos;
using App.Application.Pricing;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class CouponService(
        DataRepository<CouponEntity> couponRepo,
        DataRepository<CartItemEntity> cartItemRepo,
        TimeProvider timeProvider) : ICouponService
    {
        public async Task<ServiceResult<CouponQuoteDto>> QuoteForCartAsync(int userId, string? code)
        {
            var normalized = CouponPricing.Normalize(code);
            if (normalized.Length == 0)
                return ServiceResult<CouponQuoteDto>.ValidationError("İndirim kodunu girin.");

            if (normalized.Length > CouponPricing.MaxCodeLength)
                return ServiceResult<CouponQuoteDto>.ValidationError("Bu indirim kodu geçerli değil.");

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var cartItems = await cartItemRepo.GetAll()
                .AsNoTracking()
                .Include(ci => ci.Product.Discount)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
                return ServiceResult<CouponQuoteDto>.ValidationError("Sepetinizde ürün bulunmuyor.");

            var subtotal = cartItems.Sum(ci => ci.Quantity * ProductPricing.Apply(ci.Product.Price, ProductPricing.ActiveRate(ci.Product.Discount, now)));
            var coupon = await couponRepo.GetAll().AsNoTracking().FirstOrDefaultAsync(c => c.Code == normalized);

            var error = CouponPricing.Check(coupon, subtotal, now);
            if (error is not null)
                return ServiceResult<CouponQuoteDto>.ValidationError(error);

            return ServiceResult<CouponQuoteDto>.Ok(new CouponQuoteDto
            {
                Code = coupon!.Code,
                DiscountRate = coupon.DiscountRate,
                Subtotal = subtotal,
                DiscountAmount = CouponPricing.Discount(subtotal, coupon.DiscountRate),
            });
        }
    }
}
