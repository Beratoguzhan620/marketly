using App.Admin.Models.ViewModels;
using App.Application.Models.Requests;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController(IUserService userService, IAdminService adminService) : Controller
    {
        [Route("/users")]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AdminUserQuery query)
        {
            return View(new UserListPageViewModel
            {
                Query = query,
                Result = await adminService.GetUsersAsync(query),
            });
        }

        [Route("/users/{id:int}/approve")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSellerRequest([FromRoute] int id, [FromForm] string? returnUrl)
        {
            var result = await userService.ApproveSellerRequestAsync(id);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message ?? "Satıcı başvurusu onaylandı.";
            return Back(returnUrl);
        }

        [Route("/users/{id:int}/reject")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectSellerRequest([FromRoute] int id, [FromForm] string? returnUrl)
        {
            var result = await adminService.RejectSellerRequestAsync(id);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return Back(returnUrl);
        }

        [Route("/users/{id:int}/enable")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable([FromRoute] int id, [FromForm] string? returnUrl)
        {
            var result = await userService.SetUserEnabledAsync(id, true);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Hesap etkinleştirildi." : result.Message;
            return Back(returnUrl);
        }

        [Route("/users/{id:int}/disable")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable([FromRoute] int id, [FromForm] string? returnUrl)
        {
            if (User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value == id.ToString())
            {
                TempData["ErrorMessage"] = "Kendi hesabınızı devre dışı bırakamazsınız.";
                return Back(returnUrl);
            }

            var result = await userService.SetUserEnabledAsync(id, false);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Hesap devre dışı bırakıldı." : result.Message;
            return Back(returnUrl);
        }

        private IActionResult Back(string? returnUrl) =>
            Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(List));
    }
}
