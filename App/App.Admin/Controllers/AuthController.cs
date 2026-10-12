using App.Admin.Models.ViewModels;
using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Admin.Controllers
{
    [AllowAnonymous]
    public class AuthController(IAuthService authService) : Controller
    {
        private const string AdminRoleName = "admin";

        [Route("/login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var result = await authService.ValidateLoginAsync(new LoginRequest
            {
                Email = loginModel.Email,
                Password = loginModel.Password,
            });

            if (!result.Success || result.Data is null)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Kullanıcı adı veya şifre hatalı.");
                return View(loginModel);
            }

            if (!string.Equals(result.Data.RoleName, AdminRoleName, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Bu sayfaya erişim yetkiniz yok.");
                return View(loginModel);
            }

            await DoLoginAsync(result.Data);
            return RedirectToAction("Index", "Home");
        }

        [Route("/logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await DoLogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        private async Task DoLoginAsync(AuthUserDto user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.RoleName),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }

        private async Task DoLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
