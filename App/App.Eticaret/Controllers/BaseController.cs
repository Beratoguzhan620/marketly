using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Eticaret.Controllers
{
    public abstract class BaseController : Controller
    {
        protected void SetSuccessMessage(string message) => ViewBag.SuccessMessage = message;

        protected void SetErrorMessage(string message) => ViewBag.ErrorMessage = message;

        protected string? GetCookie(string key) => Request.Cookies[key];

        protected void SetCookie(string key, string value) => Response.Cookies.Append(key, value);

        protected void RemoveCookie(string key) => Response.Cookies.Delete(key);

        protected bool IsUserLoggedIn() => User.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Aktif kullanıcının Id bilgisini claim'ler üzerinden alır. Controller'dan gelen
        /// userId bilgisi doğrudan güvenilir kabul edilmemelidir.
        /// </summary>
        protected int? GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) ? userId : null;
    }
}
