namespace App.Application.Models.Requests
{
    public class ProductCommentCreateRequest
    {
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
    }
}
