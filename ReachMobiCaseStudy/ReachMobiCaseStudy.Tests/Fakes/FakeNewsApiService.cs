using ReachMobiCaseStudy.Models;
using ReachMobiCaseStudy.Services;

namespace ReachMobiCaseStudy.Tests.Fakes;

/// <summary>
/// In-memory stand-in for <see cref="INewsApiService"/>. Records the arguments
/// it was called with and returns a canned <see cref="NewsSearchResultsViewModel"/>
/// (or throws a configured exception) so the controller can be exercised
/// without hitting the real NewsAPI.
/// </summary>
public class FakeNewsApiService : INewsApiService
{
    private readonly NewsSearchResultsViewModel _result;
    private readonly Exception? _exceptionToThrow;

    public NewsSearchViewModel? LastModel { get; private set; }
    public int LastPage { get; private set; }
    public int LastPageSize { get; private set; }
    public int CallCount { get; private set; }

    public FakeNewsApiService(NewsSearchResultsViewModel result)
    {
        _result = result;
    }

    public FakeNewsApiService(Exception exceptionToThrow)
    {
        _result = new NewsSearchResultsViewModel();
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<NewsSearchResultsViewModel> SearchAsync(NewsSearchViewModel model, int page = 1, int pageSize = 20)
    {
        CallCount++;
        LastModel = model;
        LastPage = page;
        LastPageSize = pageSize;

        if (_exceptionToThrow is not null)
        {
            throw _exceptionToThrow;
        }

        return Task.FromResult(_result);
    }
}
