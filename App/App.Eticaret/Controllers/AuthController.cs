using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Eticaret.Controllers
{
    [AllowAnonymous]
    public class AuthController(IAuthService authService) : BaseController
    {
        [Route("/register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterUserViewModel newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            var request = new RegisterRequest
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Password = newUser.Password,
            };

            var result = await authService.RegisterAsync(request);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Kayıt işlemi başarısız oldu.");
                return View(newUser);
            }

            TempData["SuccessMessage"] = "Hesabınız oluşturuldu. Şimdi giriş yapabilirsiniz.";

            return RedirectToAction(nameof(Login));
        }

        [Route("/login")]
        [HttpGet]
        public IActionResult Login([FromQuery] string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginModel, [FromQuery] string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

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

            await LogInAsync(result.Data);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task LogInAsync(AuthUserDto user)
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

        [Route("/forgot-password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("/forgot-password")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await authService.RequestPasswordResetAsync(model.Email);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Kullanıcı bulunamadı.");
                return View(model);
            }

            ViewBag.SuccessMessage = result.Message;
            ModelState.Clear();

            return View();
        }

        [Route("/renew-password/{verificationCode}")]
        [HttpGet]
        public async Task<IActionResult> RenewPassword([FromRoute] string verificationCode)
        {
            var result = await authService.VerifyResetTokenAsync(verificationCode);

            if (!result.Success)
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new RenewPasswordViewModel { VerificationCode = verificationCode });
        }

        [Route("/renew-password")]
        [HttpPost]
        public async Task<IActionResult> RenewPassword([FromForm] RenewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await authService.ResetPasswordAsync(new RenewPasswordRequest
            {
                VerificationCode = model.VerificationCode,
                Password = model.Password,
            });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Şifre sıfırlama işlemi başarısız oldu.");
                return View(model);
            }

            ViewBag.SuccessMessage = result.Message;
            ModelState.Clear();

            return View(new RenewPasswordViewModel { VerificationCode = model.VerificationCode });
        }

        [Route("/logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }
    }
}
