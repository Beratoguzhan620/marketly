using App.Admin.Models.ViewModels;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/coupons")]
    [Authorize(Roles = "admin")]
    public class CouponController(IAdminService adminService) : Controller
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AdminCouponQuery query)
        {
            return View(new CouponListPageViewModel
            {
                Query = query,
                Result = await adminService.GetCouponsAsync(query),
            });
        }

        [Route("new")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new SaveCouponViewModel());
        }

        [Route("new")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] SaveCouponViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await adminService.CreateCouponAsync(new CouponCreateRequest
            {
                Code = model.Code,
                DiscountRate = model.DiscountRate,
                MinimumOrderTotal = model.MinimumOrderTotal,
                StartDate = model.StartDate.Date,
                // The end date is inclusive: the code works until the end of that day.
                EndDate = model.EndDate.Date.AddDays(1).AddTicks(-1),
                UsageLimit = model.UsageLimit,
            });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Kod oluşturulamadı.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(List));
        }

        [Route("bulk")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bulk([FromForm] int[] ids, [FromForm] string operation, [FromForm] string? returnUrl)
        {
            var result = operation switch
            {
                "enable" => await adminService.SetCouponsEnabledAsync(ids, true),
                "disable" => await adminService.SetCouponsEnabledAsync(ids, false),
                _ => ServiceResult.ValidationError("Bilinmeyen işlem."),
            };

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(List));
        }
    }
}
