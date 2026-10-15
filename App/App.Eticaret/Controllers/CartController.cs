using App.Application.Models.Dtos;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class CartController(ICartService cartService, ICouponService couponService) : BaseController
    {
        [HttpGet("/add-to-cart/{productId:int}")]
        public async Task<IActionResult> AddProduct([FromRoute] int productId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var result = await cartService.AddProductAsync(userId.Value, productId);

            if (!result.Success)
            {
                if (result.Type == ServiceResultType.NotFound)
                {
                    return NotFound();
                }

                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Ürün sepete eklendi.";
            }

            var prevUrl = Request.Headers.Referer.FirstOrDefault();

            // Only bounce back to pages of this site.
            if (prevUrl is null || !Uri.TryCreate(prevUrl, UriKind.Absolute, out var prevUri) || prevUri.Host != Request.Host.Host)
            {
                return RedirectToAction(nameof(Edit));
            }

            return LocalRedirect(prevUri.PathAndQuery);
        }

        [HttpGet("/cart")]
        public async Task<IActionResult> Edit()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            ApplyPreviousErrorMessage();

            var cart = await cartService.GetCartAsync(userId.Value);

            return View(MapToViewModels(cart));
        }

        [HttpPost("/cart/{cartItemId:int}/remove")]
        public async Task<IActionResult> Remove([FromRoute] int cartItemId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var result = await cartService.RemoveItemAsync(userId.Value, cartItemId);

            if (!result.Success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Ürün sepetten çıkarıldı.";
            return RedirectToAction(nameof(Edit));
        }

        [HttpPost("/cart/update")]
        public async Task<IActionResult> UpdateCart(int cartItemId, byte quantity)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var result = await cartService.UpdateItemQuantityAsync(userId.Value, cartItemId, quantity);

            if (!result.Success)
            {
                if (result.Type == ServiceResultType.NotFound)
                {
                    return NotFound();
                }

                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout([FromQuery] string? coupon)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var cart = await cartService.GetCartAsync(userId.Value);

            if (cart.Items.Count == 0)
            {
                return RedirectToAction(nameof(Edit));
            }

            if (!string.IsNullOrWhiteSpace(coupon))
            {
                var quote = await couponService.QuoteForCartAsync(userId.Value, coupon);
                if (quote.Success)
                {
                    ViewData[CheckoutViewData.CouponQuote] = quote.Data;
                }
                else
                {
                    ViewData[CheckoutViewData.CouponError] = quote.Message;
                    ViewData[CheckoutViewData.CouponInput] = coupon.Trim();
                }
            }

            return View(MapToViewModels(cart));
        }

        private void ApplyPreviousErrorMessage()
        {
            var previousErrorMessage = TempData["ErrorMessage"]?.ToString();

            if (!string.IsNullOrEmpty(previousErrorMessage))
            {
                SetErrorMessage(previousErrorMessage);
            }
        }

        private static CartItemViewModel MapToViewModel(CartItemDto item) => new()
        {
            Id = item.Id,
            ProductName = item.ProductName,
            ProductId = item.ProductId,
            CategoryId = item.CategoryId,
            CategoryName = item.CategoryName,
            ProductImage = item.ProductImage,
            Quantity = item.Quantity,
            Price = item.Price,
            ListPrice = item.ListPrice,
            DiscountRate = item.DiscountRate,
            StockAmount = item.StockAmount,
        };

        private static List<CartItemViewModel> MapToViewModels(CartDto cart) =>
            cart.Items.Select(MapToViewModel).ToList();
    }
}
