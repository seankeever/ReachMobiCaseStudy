namespace ReachMobiCaseStudy.Models;

public class NewsSearchResultsViewModel
{
    public string? Keyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<NewsArticleViewModel> Articles { get; set; } = new();
}
