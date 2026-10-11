using App.Admin.Models.ViewModels;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/comment")]
    [Authorize(Roles = "admin")]
    public class CommentController(IProductCommentService productCommentService) : Controller
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var comments = await productCommentService.GetPendingCommentsAsync();

            var viewModel = comments.Select(c => new ProductCommentViewModel
            {
                Id = c.Id,
                ProductName = c.ProductName,
                Text = c.Text,
                StarCount = c.StarCount,
                UserName = c.UserName,
                CreatedAt = c.CreatedAt,
            }).ToList();

            return View(viewModel);
        }

        [Route("{commentId:int}/approve")]
        [HttpGet]
        public async Task<IActionResult> Approve([FromRoute] int commentId)
        {
            var result = await productCommentService.ApproveCommentAsync(commentId);

            if (!result.Success)
            {
                return NotFound(new { message = result.Message });
            }

            return Json(new { success = true });
        }
    }
}
