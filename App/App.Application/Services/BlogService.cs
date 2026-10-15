using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class BlogService(
        DataRepository<BlogEntity> blogRepo,
        DataRepository<BlogCategoryEntity> categoryRepo,
        DataRepository<BlogTagEntity> tagRepo,
        DataRepository<BlogCommentEntity> commentRepo) : IBlogService
    {
        private const int ExcerptLength = 180;
        private const int WordsPerMinute = 200;
        private const int RelatedCount = 3;

        public async Task<BlogListDto> GetPostsAsync(BlogListQuery query)
        {
            var categories = await categoryRepo.GetAll()
                .AsNoTracking()
                .Select(c => new { c.Id, c.Name, PostCount = c.BlogRelations.Count(r => r.Blog.Enabled) })
                .Where(c => c.PostCount > 0)
                .OrderBy(c => c.Name)
                .Select(c => new BlogFacetDto { Id = c.Id, Name = c.Name, PostCount = c.PostCount })
                .ToListAsync();

            var tags = await tagRepo.GetAll()
                .AsNoTracking()
                .Select(t => new { t.Id, t.Name, PostCount = t.BlogRelations.Count(r => r.Blog.Enabled) })
                .Where(t => t.PostCount > 0)
                .OrderByDescending(t => t.PostCount).ThenBy(t => t.Name)
                .Select(t => new BlogFacetDto { Id = t.Id, Name = t.Name, PostCount = t.PostCount })
                .ToListAsync();

            var posts = blogRepo.GetAll().AsNoTracking().Where(b => b.Enabled);

            var activeCategory = categories.FirstOrDefault(c => c.Id == query.CategoryId);
            if (activeCategory is not null)
                posts = posts.Where(b => b.CategoryLinks.Any(l => l.CategoryId == activeCategory.Id));

            var activeTag = tags.FirstOrDefault(t => t.Id == query.TagId);
            if (activeTag is not null)
                posts = posts.Where(b => b.TagLinks.Any(l => l.TagId == activeTag.Id));

            var pageSize = Math.Clamp(query.PageSize, 1, 24);
            var total = await posts.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            var page = Math.Clamp(query.Page, 1, totalPages);

            var items = await Summaries(posts.OrderByDescending(b => b.CreatedAt).ThenByDescending(b => b.Id)
                .Skip((page - 1) * pageSize).Take(pageSize));

            return new BlogListDto
            {
                Posts = new PagedResult<BlogPostSummaryDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total },
                Categories = categories,
                Tags = tags,
                ActiveCategory = activeCategory,
                ActiveTag = activeTag,
            };
        }

        public async Task<ServiceResult<BlogPostDetailDto>> GetPostAsync(int id)
        {
            var post = await blogRepo.GetAll()
                .AsNoTracking()
                .Where(b => b.Id == id && b.Enabled)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    b.ImageUrl,
                    AuthorName = b.User.FirstName + " " + b.User.LastName,
                    b.CreatedAt,
                    Categories = b.CategoryLinks.OrderBy(l => l.Category.Name).Select(l => new BlogFacetDto { Id = l.CategoryId, Name = l.Category.Name }).ToList(),
                    Tags = b.TagLinks.OrderBy(l => l.Tag.Name).Select(l => new BlogFacetDto { Id = l.TagId, Name = l.Tag.Name }).ToList(),
                    Comments = b.Comments.Where(c => c.IsApproved).OrderBy(c => c.CreatedAt)
                        .Select(c => new BlogCommentDto { Name = c.Name, Comment = c.Comment, CreatedAt = c.CreatedAt }).ToList(),
                })
                .FirstOrDefaultAsync();

            if (post is null)
                return ServiceResult<BlogPostDetailDto>.NotFound("Yazı bulunamadı.");

            var categoryIds = post.Categories.Select(c => c.Id).ToList();
            var related = await Summaries(blogRepo.GetAll().AsNoTracking()
                .Where(b => b.Enabled && b.Id != id)
                .OrderByDescending(b => b.CategoryLinks.Count(l => categoryIds.Contains(l.CategoryId)))
                .ThenByDescending(b => b.CreatedAt)
                .Take(RelatedCount));

            return ServiceResult<BlogPostDetailDto>.Ok(new BlogPostDetailDto
            {
                Id = post.Id,
                Title = post.Title,
                Paragraphs = SplitParagraphs(post.Content),
                ImageUrl = post.ImageUrl,
                AuthorName = post.AuthorName,
                CreatedAt = post.CreatedAt,
                ReadingMinutes = ReadingMinutes(post.Content),
                Categories = post.Categories,
                Tags = post.Tags,
                Comments = post.Comments,
                Related = related,
            });
        }

        public async Task<ServiceResult> AddCommentAsync(BlogCommentCreateRequest request)
        {
            var name = request.Name?.Trim() ?? string.Empty;
            var email = request.Email?.Trim() ?? string.Empty;
            var text = request.Comment?.Trim() ?? string.Empty;

            if (name.Length is < 2 or > 100)
                return ServiceResult.ValidationError("Adınız 2 ile 100 karakter arasında olmalı.");

            if (email.Length is 0 or > 100 || !email.Contains('@'))
                return ServiceResult.ValidationError("Geçerli bir e-posta adresi girin.");

            if (text.Length is < 3 or > 1000)
                return ServiceResult.ValidationError("Yorum 3 ile 1000 karakter arasında olmalı.");

            if (!await blogRepo.GetAll().AnyAsync(b => b.Id == request.BlogId && b.Enabled))
                return ServiceResult.NotFound("Yazı bulunamadı.");

            await commentRepo.AddAsync(new BlogCommentEntity
            {
                BlogId = request.BlogId,
                Name = name,
                Email = email,
                Comment = text,
                IsApproved = false,
            });

            return ServiceResult.Ok("Yorumunuz alındı. Onaylandıktan sonra yazının altında görünecek.");
        }

        private static async Task<List<BlogPostSummaryDto>> Summaries(IQueryable<BlogEntity> posts)
        {
            var rows = await posts
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    b.ImageUrl,
                    AuthorName = b.User.FirstName + " " + b.User.LastName,
                    b.CreatedAt,
                    CommentCount = b.Comments.Count(c => c.IsApproved),
                    Categories = b.CategoryLinks.OrderBy(l => l.Category.Name).Select(l => new BlogFacetDto { Id = l.CategoryId, Name = l.Category.Name }).ToList(),
                })
                .ToListAsync();

            return rows.Select(r => new BlogPostSummaryDto
            {
                Id = r.Id,
                Title = r.Title,
                Excerpt = Excerpt(r.Content),
                ImageUrl = r.ImageUrl,
                AuthorName = r.AuthorName,
                CreatedAt = r.CreatedAt,
                ReadingMinutes = ReadingMinutes(r.Content),
                CommentCount = r.CommentCount,
                Categories = r.Categories,
            }).ToList();
        }

        public static List<string> SplitParagraphs(string content) =>
            content.Replace("\r\n", "\n")
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

        public static string Excerpt(string content)
        {
            var first = SplitParagraphs(content).FirstOrDefault() ?? string.Empty;
            if (first.Length <= ExcerptLength)
                return first;

            var cut = first.LastIndexOf(' ', ExcerptLength);
            return first[..(cut > 0 ? cut : ExcerptLength)].TrimEnd(',', '.', ';', ':') + "…";
        }

        public static int ReadingMinutes(string content)
        {
            var words = content.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
            return Math.Max(1, (int)Math.Ceiling(words / (double)WordsPerMinute));
        }
    }
}
