namespace App.Application.Options
{
    /// <summary>
    /// SMTP settings for transactional e-mails (e.g. password reset). Real credentials come
    /// from User Secrets or environment variables (Email__Host, Email__Username, ...).
    /// When <see cref="Host"/> is empty, e-mails are not sent and the link is written to the log instead.
    /// </summary>
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "Marketly";

        /// <summary>Public URL of the storefront, used to build links inside e-mails.</summary>
        public string StorefrontUrl { get; set; } = "https://localhost:7114";

        public bool IsConfigured => !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(FromAddress);
    }
}
