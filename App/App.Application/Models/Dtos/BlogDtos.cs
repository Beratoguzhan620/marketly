namespace App.Application.Models.Dtos
{
    public class BlogFacetDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public int PostCount { get; init; }
    }

    public class BlogPostSummaryDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = null!;
        public string Excerpt { get; init; } = null!;
        public string? ImageUrl { get; init; }
        public string AuthorName { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public int ReadingMinutes { get; init; }
        public int CommentCount { get; init; }
        public List<BlogFacetDto> Categories { get; init; } = [];
    }

    public class BlogListDto
    {
        public PagedResult<BlogPostSummaryDto> Posts { get; init; } = new();
        public List<BlogFacetDto> Categories { get; init; } = [];
        public List<BlogFacetDto> Tags { get; init; } = [];
        public BlogFacetDto? ActiveCategory { get; init; }
        public BlogFacetDto? ActiveTag { get; init; }
    }

    public class BlogCommentDto
    {
        public string Name { get; init; } = null!;
        public string Comment { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
    }

    public class BlogPostDetailDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = null!;
        public List<string> Paragraphs { get; init; } = [];
        public string? ImageUrl { get; init; }
        public string AuthorName { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public int ReadingMinutes { get; init; }
        public List<BlogFacetDto> Categories { get; init; } = [];
        public List<BlogFacetDto> Tags { get; init; } = [];
        public List<BlogCommentDto> Comments { get; init; } = [];
        public List<BlogPostSummaryDto> Related { get; init; } = [];
    }

    public class AdminBlogCommentRowDto
    {
        public int Id { get; init; }
        public int BlogId { get; init; }
        public string BlogTitle { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Comment { get; init; } = null!;
        public bool IsApproved { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
