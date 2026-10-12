using System.Security.Cryptography;
using System.Text;

namespace App.Application.Security
{
    /// <summary>
    /// PBKDF2 (HMAC-SHA256) password hashing. Stored format:
    /// <c>PBKDF2$SHA256${iterations}${base64 salt}${base64 hash}</c>, so the
    /// iteration count can be raised later without breaking existing hashes.
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2";
        private const string Algorithm = "SHA256";
        private const int SaltSize = 16;
        private const int KeySize = 32;
        public const int DefaultIterations = 100_000;

        public static string Hash(string password, int iterations = DefaultIterations)
        {
            ArgumentException.ThrowIfNullOrEmpty(password);

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Derive(password, salt, iterations);

            return string.Join('$', Prefix, Algorithm, iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
        }

        public static bool Verify(string password, string? storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            var parts = storedHash.Split('$');
            if (parts.Length != 5
                || parts[0] != Prefix
                || parts[1] != Algorithm
                || !int.TryParse(parts[2], out var iterations)
                || iterations <= 0)
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[3]);
                var expected = Convert.FromBase64String(parts[4]);
                var actual = Derive(password, salt, iterations, expected.Length);

                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int length = KeySize) =>
            Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, length);
    }
}
