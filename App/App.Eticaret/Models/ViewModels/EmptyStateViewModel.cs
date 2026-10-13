namespace App.Eticaret.Models.ViewModels
{
    public record EmptyStateViewModel(string Title, string Description, string Icon = "bi-inbox", string? ActionText = null, string? ActionUrl = null);
}
