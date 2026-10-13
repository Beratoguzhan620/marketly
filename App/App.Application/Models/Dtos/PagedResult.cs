namespace App.Application.Models.Dtos
{
    public class PagedResult<T>
    {
        public List<T> Items { get; init; } = [];
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
        public int FirstItemNumber => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int LastItemNumber => Math.Min(Page * PageSize, TotalCount);
    }
}
