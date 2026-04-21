namespace ReachMobiCaseStudy.Models;

public class ArticleClickStatsViewModel
{
    public List<ArticleClickItemViewModel> Articles { get; set; } = new();
    public int TotalClicks { get; set; }
}
