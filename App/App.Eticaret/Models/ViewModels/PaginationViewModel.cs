namespace App.Eticaret.Models.ViewModels
{
    /// <summary>Page links that keep the current query string and only change <c>page</c>.</summary>
    public class PaginationViewModel
    {
        public int Page { get; init; }
        public int TotalPages { get; init; }
        public Func<int, string> PageUrl { get; init; } = _ => "#";

        /// <summary>Page numbers to show; null stands for a gap.</summary>
        public IEnumerable<int?> Window()
        {
            if (TotalPages <= 7)
            {
                for (var i = 1; i <= TotalPages; i++) yield return i;
                yield break;
            }

            var pages = new SortedSet<int> { 1, TotalPages, Page - 1, Page, Page + 1 };
            int? previous = null;
            foreach (var p in pages.Where(p => p >= 1 && p <= TotalPages))
            {
                if (previous is not null && p - previous > 1) yield return null;
                yield return p;
                previous = p;
            }
        }
    }
}
