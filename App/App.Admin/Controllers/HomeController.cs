using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class HomeController(IAdminService adminService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await adminService.GetDashboardAsync());
        }

        [AllowAnonymous]
        [Route("/not-found")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("NotFound");
        }
    }
}
