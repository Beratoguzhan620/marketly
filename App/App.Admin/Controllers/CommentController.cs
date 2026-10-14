using App.Admin.Models.ViewModels;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/comments")]
    [Authorize(Roles = "admin")]
    public class CommentController(IAdminService adminService) : Controller
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AdminCommentQuery query)
        {
            return View(new CommentListPageViewModel
            {
                Query = query,
                Result = await adminService.GetCommentsAsync(query),
            });
        }

        [Route("bulk")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bulk([FromForm] int[] ids, [FromForm] string operation, [FromForm] string? returnUrl)
        {
            var result = operation switch
            {
                "approve" => await adminService.ApproveCommentsAsync(ids),
                "delete" => await adminService.DeleteCommentsAsync(ids),
                _ => ServiceResult.ValidationError("Bilinmeyen işlem."),
            };

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(List));
        }
    }
}
