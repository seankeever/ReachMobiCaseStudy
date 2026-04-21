namespace ReachMobiCaseStudy.Models;

public class NewsSearchResultsViewModel
{
    public string? Keyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<NewsArticleViewModel> Articles { get; set; } = new();

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalResults { get; set; }

    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling((double)TotalResults / PageSize)
        : 0;
}
