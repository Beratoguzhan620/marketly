namespace App.Application.Models.Requests
{
    public abstract class AdminPagedQuery
    {
        public const int DefaultPageSize = 15;

        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
    }

    public enum ActiveFilter
    {
        All,
        Active,
        Inactive,
    }

    public class AdminProductQuery : AdminPagedQuery
    {
        public int? CategoryId { get; set; }
        public ActiveFilter Status { get; set; }
    }

    public class AdminUserQuery : AdminPagedQuery
    {
        /// <summary>admin, seller or buyer; null for every role.</summary>
        public string? Role { get; set; }
        public ActiveFilter Status { get; set; }
        public bool OnlySellerRequests { get; set; }
    }

    public enum CommentStatusFilter
    {
        Pending,
        Approved,
        All,
    }

    public class AdminCommentQuery : AdminPagedQuery
    {
        public CommentStatusFilter Status { get; set; }
    }
}
