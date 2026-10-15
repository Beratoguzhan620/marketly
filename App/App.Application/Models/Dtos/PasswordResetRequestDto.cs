namespace App.Application.Models.Dtos
{
    public class PasswordResetRequestDto
    {
        /// <summary>
        /// Set only when the e-mail could not be sent (no SMTP configured). The storefront shows it
        /// in the Development environment so the flow can be tried locally.
        /// </summary>
        public string? UndeliveredLink { get; set; }
    }
}
