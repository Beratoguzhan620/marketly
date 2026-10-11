namespace App.Application.Models.Dtos
{
    public class ProductCommentDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public string UserName { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
