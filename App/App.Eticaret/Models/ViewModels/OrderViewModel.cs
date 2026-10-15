using App.Data.Entities;

namespace App.Eticaret.Models.ViewModels
{
    public class OrderViewModel
    {
        public string OrderCode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
    }
}