namespace ReachMobiCaseStudy.Models;

public class NewsArticleViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? UrlToImage { get; set; }
    public string? SourceName { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? Author { get; set; }
}
