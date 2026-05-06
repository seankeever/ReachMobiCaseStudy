using Microsoft.AspNetCore.Mvc;
using ReachMobiCaseStudy.Controllers;
using ReachMobiCaseStudy.Models;
using ReachMobiCaseStudy.Tests.Fakes;
using Xunit;

namespace ReachMobiCaseStudy.Tests;

public class NewsControllerTests
{
    [Fact]
    public async Task Search_WithKeyword_ReturnsResultsViewPopulatedFromService()
    {
        // Arrange — canned response that the fake service will hand back
        var canned = new NewsSearchResultsViewModel
        {
            Keyword = "sports",
            Page = 1,
            PageSize = 20,
            TotalResults = 1,
            Articles = new List<NewsArticleViewModel>
            {
                new()
                {
                    Title = "Test article",
                    Url = "https://example.com/article",
                    SourceName = "Example",
                    PublishedAt = new DateTime(2026, 1, 15)
                }
            }
        };

        var fakeService = new FakeNewsApiService(canned);
        var controller = new NewsController(fakeService, new FakeSessionArticleTracker());

        // Act
        var result = await controller.Search(
            keyword: "sports",
            fromDate: null,
            toDate: null,
            exactPhrase: null,
            excludeWords: null,
            titleOnly: null,
            domains: null,
            excludeDomains: null,
            sortBy: null);

        // Assert — the controller should render the Results view with the model the fake gave it
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Results", viewResult.ViewName);

        var model = Assert.IsType<NewsSearchResultsViewModel>(viewResult.Model);
        Assert.Single(model.Articles);
        Assert.Equal("Test article", model.Articles[0].Title);

        // And it should have called the service exactly once with the search args we passed in
        Assert.Equal(1, fakeService.CallCount);
        Assert.Equal("sports", fakeService.LastModel?.Keyword);
        Assert.Equal(1, fakeService.LastPage);
        Assert.Equal(20, fakeService.LastPageSize);
        // SortBy should default to "publishedAt" when none is supplied
        Assert.Equal("publishedAt", fakeService.LastModel?.SortBy);
    }

    [Fact]
    public async Task Search_WithNoSearchInput_ReturnsIndexViewWithModelStateError_AndNeverCallsService()
    {
        var fakeService = new FakeNewsApiService(new NewsSearchResultsViewModel());
        var controller = new NewsController(fakeService, new FakeSessionArticleTracker());

        var result = await controller.Search(
            keyword: null, fromDate: null, toDate: null, exactPhrase: null,
            excludeWords: null, titleOnly: null, domains: null,
            excludeDomains: null, sortBy: null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal(0, fakeService.CallCount);
    }

    [Fact]
    public async Task Search_WhenFromDateAfterToDate_ReturnsIndexViewWithModelStateError_AndNeverCallsService()
    {
        var fakeService = new FakeNewsApiService(new NewsSearchResultsViewModel());
        var controller = new NewsController(fakeService, new FakeSessionArticleTracker());

        var result = await controller.Search(
            keyword: "sports",
            fromDate: new DateTime(2026, 5, 1),
            toDate: new DateTime(2026, 4, 1),
            exactPhrase: null, excludeWords: null, titleOnly: null,
            domains: null, excludeDomains: null, sortBy: null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal(0, fakeService.CallCount);
    }

    [Fact]
    public async Task Search_WhenServiceThrowsRateLimited_ShowsFriendlyErrorOnIndex()
    {
        // The controller inspects the exception message to surface a friendly error.
        var fakeService = new FakeNewsApiService(new InvalidOperationException("rateLimited: too many requests"));
        var controller = new NewsController(fakeService, new FakeSessionArticleTracker());

        var result = await controller.Search(
            keyword: "sports",
            fromDate: null, toDate: null, exactPhrase: null,
            excludeWords: null, titleOnly: null, domains: null,
            excludeDomains: null, sortBy: null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);

        var model = Assert.IsType<NewsSearchViewModel>(viewResult.Model);
        Assert.NotNull(model.ErrorMessage);
        Assert.Contains("rate limit", model.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
