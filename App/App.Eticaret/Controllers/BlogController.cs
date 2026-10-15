using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Eticaret.Controllers
{
    public class BlogController(IBlogService blogService) : BaseController
    {
        [HttpGet("blog")]
        public async Task<IActionResult> Index([FromQuery] int? category, [FromQuery] int? tag, [FromQuery] int page = 1)
        {
            var model = await blogService.GetPostsAsync(new BlogListQuery
            {
                CategoryId = category,
                TagId = tag,
                Page = page,
            });

            return View(model);
        }

        [HttpGet("blog/{id:int}")]
        public async Task<IActionResult> Detail([FromRoute] int id)
        {
            var result = await blogService.GetPostAsync(id);

            if (!result.Success || result.Data is null)
            {
                return NotFound();
            }

            return View(new BlogDetailPageViewModel
            {
                Post = result.Data,
                Form = new BlogCommentFormViewModel
                {
                    Name = IsUserLoggedIn() ? $"{User.FindFirstValue(ClaimTypes.Name)} {User.FindFirstValue(ClaimTypes.Surname)}".Trim() : string.Empty,
                    Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                },
            });
        }

        [HttpPost("blog/{id:int}/comments")]
        public async Task<IActionResult> AddComment([FromRoute] int id, [Bind(Prefix = "Form")] BlogCommentFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await blogService.AddCommentAsync(new BlogCommentCreateRequest
                {
                    BlogId = id,
                    Name = form.Name,
                    Email = form.Email,
                    Comment = form.Comment,
                });

                if (result.Type == ServiceResultType.NotFound)
                {
                    return NotFound();
                }

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return Redirect(Url.Action(nameof(Detail), new { id }) + "#comments");
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Yorum gönderilemedi.");
            }

            var post = await blogService.GetPostAsync(id);
            if (!post.Success || post.Data is null)
            {
                return NotFound();
            }

            return View(nameof(Detail), new BlogDetailPageViewModel { Post = post.Data, Form = form });
        }
    }
}
