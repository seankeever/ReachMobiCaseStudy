namespace ReachMobiCaseStudy.Models;

public class NewsSearchResultsViewModel
{
    public string? Keyword { get; set; }
    public DateTime? Date { get; set; }
    public List<NewsArticleViewModel> Articles { get; set; } = new();
}
