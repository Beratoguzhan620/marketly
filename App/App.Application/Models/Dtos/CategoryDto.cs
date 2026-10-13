namespace App.Application.Models.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string IconCssClass { get; set; } = null!;

        /// <summary>Number of products on sale in this category.</summary>
        public int ProductCount { get; set; }
    }
}
