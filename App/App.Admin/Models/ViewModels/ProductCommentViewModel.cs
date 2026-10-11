namespace App.Admin.Models.ViewModels
{
    public class ProductCommentViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
