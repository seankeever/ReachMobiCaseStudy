using ReachMobiCaseStudy.Models;

namespace ReachMobiCaseStudy.Services;

public interface INewsApiService
{
    Task<NewsSearchResultsViewModel> SearchAsync(NewsSearchViewModel model, int page = 1, int pageSize = 20);
}
