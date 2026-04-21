using ReachMobiCaseStudy.Models;

namespace ReachMobiCaseStudy.Services;

public interface INewsApiService
{
    Task<NewsSearchResultsViewModel> SearchAsync(string? keyword, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20);
}
