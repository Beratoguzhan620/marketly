using App.Application.Models.Requests;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "seller, buyer")]
    public class ProfileController(IUserService userService, IOrderService orderService, IProductService productService) : BaseController
    {
        [HttpGet("/profile")]
        public async Task<IActionResult> Details()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await userService.GetProfileAsync(userId.Value);

            if (!result.Success || result.Data is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userViewModel = new ProfileDetailsViewModel
            {
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
            };

            string? previousSuccessMessage = TempData["SuccessMessage"]?.ToString();

            if (previousSuccessMessage is not null)
            {
                SetSuccessMessage(previousSuccessMessage);
            }

            return View(userViewModel);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> Edit([FromForm] ProfileDetailsViewModel editMyProfileModel)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(nameof(Details), editMyProfileModel);
            }

            var result = await userService.UpdateProfileAsync(userId.Value, new UserUpdateRequest
            {
                FirstName = editMyProfileModel.FirstName,
                LastName = editMyProfileModel.LastName,
                Password = editMyProfileModel.Password,
            });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Profil güncellenemedi.");
                return View(nameof(Details), editMyProfileModel);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction(nameof(Details));
        }

        [HttpGet("/become-seller")]
        public async Task<IActionResult> RequestSellerRole()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await userService.RequestSellerRoleAsync(userId.Value);

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction(nameof(Details));
        }

        [HttpGet("/my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await orderService.GetMyOrdersAsync(userId.Value);

            var viewModel = orders.Select(o => new OrderViewModel
            {
                OrderCode = o.OrderCode,
                Address = o.Address,
                CreatedAt = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                TotalProducts = o.TotalProducts,
                TotalQuantity = o.TotalQuantity,
            }).ToList();

            return View(viewModel);
        }

        [HttpGet("/my-products")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> MyProducts()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var products = await productService.GetMyProductsAsync(userId.Value);

            var viewModel = products.Select(p => new MyProductsViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock,
                HasDiscount = p.HasDiscount,
                CreatedAt = p.CreatedAt,
            }).ToList();

            return View(viewModel);
        }
    }
}
