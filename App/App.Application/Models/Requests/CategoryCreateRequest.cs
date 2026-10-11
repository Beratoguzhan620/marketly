namespace App.Application.Models.Requests
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string? IconCssClass { get; set; }
    }
}
