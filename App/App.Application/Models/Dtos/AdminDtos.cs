namespace App.Application.Models.Dtos
{
    public class AdminDashboardDto
    {
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public int UserCount { get; set; }
        public int NewUsersLast30Days { get; set; }
        public int ActiveProductCount { get; set; }
        public int PendingCommentCount { get; set; }
        public int PendingSellerRequestCount { get; set; }
        public List<MonthlySalesDto> MonthlySales { get; set; } = [];
        public List<AdminOrderSummaryDto> RecentOrders { get; set; } = [];
    }

    public class MonthlySalesDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class AdminOrderSummaryDto
    {
        public string OrderCode { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
    }

    public class AdminProductRowDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string SellerName { get; set; } = null!;
        public decimal Price { get; set; }
        public byte Stock { get; set; }
        public bool Enabled { get; set; }
        public int ImageCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminUserRowDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool Enabled { get; set; }
        public bool HasSellerRequest { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminCommentRowDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
