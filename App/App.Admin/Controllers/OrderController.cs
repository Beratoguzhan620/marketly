using App.Admin.Models.ViewModels;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/orders")]
    [Authorize(Roles = "admin")]
    public class OrderController(IAdminService adminService) : Controller
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AdminOrderQuery query)
        {
            return View(new OrderListPageViewModel
            {
                Query = query,
                Result = await adminService.GetOrdersAsync(query),
            });
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> Detail([FromRoute] int id)
        {
            var order = await adminService.GetOrderAsync(id);
            return order is null ? NotFound() : View(order);
        }

        [Route("{id:int}/status")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromForm] OrderStatus status, [FromForm] string? returnUrl)
        {
            var result = await adminService.UpdateOrderStatusAsync(id, status);

            if (result.Type == ServiceResultType.NotFound)
            {
                return NotFound();
            }

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(Detail), new { id });
        }
    }
}
