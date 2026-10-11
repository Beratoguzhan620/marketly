namespace App.Application.Models.Requests
{
    public class UserUpdateRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Null veya boş bırakılırsa şifre değiştirilmez.
        /// </summary>
        public string? Password { get; set; }
    }
}
