using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Validation
{
    /// <summary>
    /// IFormFile listesi için server-side dosya sayısı, boyutu ve uzantı doğrulaması yapar.
    /// Gerçek yükleme File API tarafında tekrar (uzantı + ContentType + imza) doğrulanır;
    /// bu attribute yalnızca kullanıcıya erken ve anlaşılır bir hata mesajı göstermek içindir.
    /// </summary>
    public class ValidImagesAttribute(long maxFileSizeBytes = 5 * 1024 * 1024, int maxFileCount = 5) : ValidationAttribute
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IList<IFormFile> files || files.Count == 0)
            {
                return ValidationResult.Success;
            }

            if (files.Count > maxFileCount)
            {
                return new ValidationResult($"En fazla {maxFileCount} görsel yükleyebilirsiniz.");
            }

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    return new ValidationResult("Boş dosya yüklenemez.");
                }

                if (file.Length > maxFileSizeBytes)
                {
                    return new ValidationResult($"Her görsel en fazla {maxFileSizeBytes / (1024 * 1024)} MB olabilir.");
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                {
                    return new ValidationResult("Yalnızca jpg, jpeg, png veya webp uzantılı görseller yüklenebilir.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
