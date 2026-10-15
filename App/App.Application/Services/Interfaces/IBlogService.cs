using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Results;

namespace App.Application.Services.Interfaces
{
    public interface IBlogService
    {
        Task<BlogListDto> GetPostsAsync(BlogListQuery query);

        Task<ServiceResult<BlogPostDetailDto>> GetPostAsync(int id);

        /// <summary>Stores a reader comment; it stays hidden until an admin approves it.</summary>
        Task<ServiceResult> AddCommentAsync(BlogCommentCreateRequest request);
    }
}
