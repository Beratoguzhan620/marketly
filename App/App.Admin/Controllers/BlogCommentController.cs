using App.Admin.Models.ViewModels;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/blog-comments")]
    [Authorize(Roles = "admin")]
    public class BlogCommentController(IAdminService adminService) : Controller
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AdminBlogCommentQuery query)
        {
            return View(new BlogCommentListPageViewModel
            {
                Query = query,
                Result = await adminService.GetBlogCommentsAsync(query),
            });
        }

        [Route("bulk")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bulk([FromForm] int[] ids, [FromForm] string operation, [FromForm] string? returnUrl)
        {
            var result = operation switch
            {
                "approve" => await adminService.ApproveBlogCommentsAsync(ids),
                "delete" => await adminService.DeleteBlogCommentsAsync(ids),
                _ => ServiceResult.ValidationError("Bilinmeyen işlem."),
            };

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(List));
        }
    }
}
