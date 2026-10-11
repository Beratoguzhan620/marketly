using App.Admin.Models.ViewModels;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController(IUserService userService) : Controller
    {
        [Route("/users")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await userService.GetUsersAsync();

            var viewModel = (result.Data ?? []).Select(u => new UserListItemViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                Enabled = u.Enabled,
                HasSellerRequest = u.HasSellerRequest
            }).ToList();

            return View(viewModel);
        }

        [Route("/users/{id:int}/approve")]
        [HttpGet]
        public async Task<IActionResult> ApproveSellerRequest([FromRoute] int id)
        {
            var result = await userService.ApproveSellerRequestAsync(id);

            if (!result.Success)
            {
                return result.Type switch
                {
                    ServiceResultType.NotFound => NotFound(),
                    _ => BadRequest(result.Message)
                };
            }

            return RedirectToAction(nameof(List));
        }

        [Route("/users/{id:int}/enable")]
        public async Task<IActionResult> Enable([FromRoute] int id)
        {
            var result = await userService.SetUserEnabledAsync(id, true);

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(List));
        }

        [Route("/users/{id:int}/disable")]
        public async Task<IActionResult> Disable([FromRoute] int id)
        {
            var result = await userService.SetUserEnabledAsync(id, false);

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(List));
        }
    }
}
