using ReachMobiCaseStudy.Models;

namespace ReachMobiCaseStudy.Services;

public interface INewsApiService
{
    Task<List<NewsArticleViewModel>> SearchAsync(string? keyword, DateTime? date);
}
