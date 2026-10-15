using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class OrderController(IOrderService orderService, ICartService cartService, ICouponService couponService) : BaseController
    {
        [HttpPost("/order")]
        public async Task<IActionResult> Create([FromForm] CheckoutViewModel model)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            if (!ModelState.IsValid)
            {
                return await CheckoutAgainAsync(userId.Value, model.CouponCode);
            }

            var result = await orderService.CreateOrderAsync(userId.Value, new OrderCreateRequest
            {
                Address = model.Address,
                CouponCode = model.CouponCode,
            });

            if (!result.Success || result.Data is null)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Sipariş oluşturulamadı.");
                return await CheckoutAgainAsync(userId.Value, model.CouponCode);
            }

            return RedirectToAction(nameof(Details), new { orderCode = result.Data.OrderCode, placed = true });
        }

        [HttpGet("/order/{orderCode}/details")]
        public async Task<IActionResult> Details([FromRoute] string orderCode)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var result = await orderService.GetOrderDetailAsync(userId.Value, orderCode);

            if (!result.Success || result.Data is null)
            {
                return NotFound();
            }

            var order = result.Data;

            var viewModel = new OrderDetailsViewModel
            {
                OrderCode = order.OrderCode,
                CreatedAt = order.CreatedAt,
                Address = order.Address,
                Status = order.Status,
                StatusUpdatedAt = order.StatusUpdatedAt,
                CouponCode = order.CouponCode,
                DiscountAmount = order.DiscountAmount,
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };

            return View(viewModel);
        }

        private async Task<IActionResult> CheckoutAgainAsync(int userId, string? couponCode)
        {
            var cart = await cartService.GetCartAsync(userId);
            if (cart.Items.Count == 0)
            {
                return RedirectToAction(nameof(CartController.Edit), "Cart");
            }

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                var quote = await couponService.QuoteForCartAsync(userId, couponCode);
                if (quote.Success)
                {
                    ViewData[CheckoutViewData.CouponQuote] = quote.Data;
                }
            }

            return View("~/Views/Cart/Checkout.cshtml", MapToViewModels(cart));
        }

        private static List<CartItemViewModel> MapToViewModels(CartDto cart) =>
            cart.Items.Select(i => new CartItemViewModel
            {
                Id = i.Id,
                ProductName = i.ProductName,
                ProductId = i.ProductId,
                CategoryId = i.CategoryId,
                CategoryName = i.CategoryName,
                ProductImage = i.ProductImage,
                Quantity = i.Quantity,
                Price = i.Price,
                ListPrice = i.ListPrice,
                DiscountRate = i.DiscountRate,
                StockAmount = i.StockAmount,
            }).ToList();
    }
}
