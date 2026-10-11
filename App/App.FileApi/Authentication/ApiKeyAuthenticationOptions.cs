using Microsoft.AspNetCore.Authentication;

namespace App.FileApi.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string SchemeName = "ApiKey";
        public const string HeaderName = "X-Api-Key";

        /// <summary>
        /// Beklenen API Key değeri. Program.cs içerisinde configuration (User Secrets/environment variable)
        /// üzerinden okunarak set edilir; kaynak koda gömülmez.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
