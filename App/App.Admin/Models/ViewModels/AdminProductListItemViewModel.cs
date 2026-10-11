namespace App.Admin.Models.ViewModels
{
    public class AdminProductListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string SellerName { get; set; } = null!;
        public decimal Price { get; set; }
        public bool Enabled { get; set; }
        public string? ImageUrl { get; set; }
    }
}
