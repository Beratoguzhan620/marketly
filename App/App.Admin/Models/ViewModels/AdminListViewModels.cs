using App.Application.Models.Dtos;
using App.Application.Models.Requests;

namespace App.Admin.Models.ViewModels
{
    public class AdminListViewModel<TRow, TQuery> where TQuery : AdminPagedQuery
    {
        public PagedResult<TRow> Result { get; set; } = new();
        public TQuery Query { get; set; } = default!;

        public PagerViewModel Pager => new(Result.Page, Result.PageSize, Result.TotalCount, Result.TotalPages);
    }

    public class ProductListPageViewModel : AdminListViewModel<AdminProductRowDto, AdminProductQuery>
    {
        public List<CategoryDto> Categories { get; set; } = [];
    }

    public class UserListPageViewModel : AdminListViewModel<AdminUserRowDto, AdminUserQuery>
    {
    }

    public class CommentListPageViewModel : AdminListViewModel<AdminCommentRowDto, AdminCommentQuery>
    {
    }
}
