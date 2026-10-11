using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.FileApi.Authentication
{
    /// <summary>
    /// İstemcilerin "X-Api-Key" header'ı üzerinden kimliğini doğrulayan basit authentication handler.
    /// Üretim ortamında bu API'nin yalnızca yetkili istemciler (MVC uygulamaları) tarafından çağrılmasını sağlar.
    /// </summary>
    public class ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (string.IsNullOrEmpty(Options.ApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key yapılandırılmamış."));
            }

            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var providedKey))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!string.Equals(providedKey.ToString(), Options.ApiKey, StringComparison.Ordinal))
            {
                return Task.FromResult(AuthenticateResult.Fail("Geçersiz API Key."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "FileApiClient") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
