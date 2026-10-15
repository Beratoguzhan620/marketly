namespace App.Application.Models.Requests
{
    public class BlogListQuery
    {
        public const int DefaultPageSize = 6;

        public int? CategoryId { get; set; }
        public int? TagId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
    }

    public class BlogCommentCreateRequest
    {
        public int BlogId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Comment { get; set; } = null!;
    }
}
