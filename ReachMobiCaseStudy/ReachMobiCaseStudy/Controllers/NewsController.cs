using Microsoft.AspNetCore.Mvc;
using ReachMobiCaseStudy.Models;
using ReachMobiCaseStudy.Services;

namespace ReachMobiCaseStudy.Controllers;

public class NewsController : Controller
{
    private readonly INewsApiService _newsApiService;
    private readonly ISessionArticleTracker _sessionArticleTracker;

    public NewsController(INewsApiService newsApiService, ISessionArticleTracker sessionArticleTracker)
    {
        _newsApiService = newsApiService;
        _sessionArticleTracker = sessionArticleTracker;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new NewsSearchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(NewsSearchViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Keyword) && !model.Date.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Enter a keyword, a date, or both.");
        }

        if (model.Date.HasValue && model.Date.Value.Date > DateTime.UtcNow.Date)
        {
            ModelState.AddModelError(nameof(model.Date), "Date cannot be in the future.");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var articles = await _newsApiService.SearchAsync(model.Keyword, model.Date);

        var results = new NewsSearchResultsViewModel
        {
            Keyword = model.Keyword,
            Date = model.Date,
            Articles = articles
        };

        return View("Results", results);
    }

    [HttpGet]
    public IActionResult Read(string url, string title)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            return BadRequest("Invalid article URL.");
        }

        _sessionArticleTracker.TrackClick(HttpContext.Session, url, title);
        return Redirect(url);
    }

    [HttpGet]
    public IActionResult Stats()
    {
        var stats = _sessionArticleTracker.GetStats(HttpContext.Session);
        return View(stats);
    }
}
