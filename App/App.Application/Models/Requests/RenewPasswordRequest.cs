namespace App.Application.Models.Requests
{
    public class RenewPasswordRequest
    {
        public string VerificationCode { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
