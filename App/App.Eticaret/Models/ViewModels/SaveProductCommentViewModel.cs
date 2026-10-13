using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class SaveProductCommentViewModel
    {
        [Display(Name = "Yorum")]
        [Required, MinLength(5), MaxLength(500)]
        public string Text { get; set; } = null!;

        [Display(Name = "Puan")]
        [Required, Range(1, 5)]
        public byte StarCount { get; set; }
    }
}