namespace App.Eticaret.Models.ViewModels
{
    /// <summary>Product image or, when the product has none, a tinted placeholder.</summary>
    public record ProductMediaViewModel(string? ImageUrl, string ProductName, int CategoryId, string? CategoryName, string CssClass = "");
}
