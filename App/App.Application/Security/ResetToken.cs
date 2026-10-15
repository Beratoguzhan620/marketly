using System.Security.Cryptography;
using System.Text;

namespace App.Application.Security
{
    /// <summary>
    /// Password reset tokens: 32 random bytes sent to the user as hex, stored only as a SHA-256 hash
    /// so a leaked database row cannot be used to reset the password.
    /// </summary>
    public static class ResetToken
    {
        private const int ByteLength = 32;

        public static string Create() => Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(ByteLength));

        public static string Hash(string token) =>
            Convert.ToHexStringLower(SHA256.HashData(Encoding.ASCII.GetBytes(token.ToLowerInvariant())));

        public static bool LooksValid(string? token) =>
            token is { Length: ByteLength * 2 } && token.All(Uri.IsHexDigit);
    }
}
