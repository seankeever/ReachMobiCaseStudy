using Microsoft.AspNetCore.Mvc;
using ReachMobiCaseStudy.Models;
using ReachMobiCaseStudy.Services;

namespace ReachMobiCaseStudy.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsApiService _newsApiService;
        private readonly ISessionArticleTracker _sessionArticleTracker;

        public NewsController(
            INewsApiService newsApiService,
            ISessionArticleTracker sessionArticleTracker)
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
        public async Task<IActionResult> Search(NewsSearchViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Keyword))
            {
                ModelState.AddModelError(string.Empty, "Please enter a keyword. You can optionally add a date range.");
                return View("Index", model);
            }

            if (model.FromDate.HasValue && model.ToDate.HasValue && model.FromDate > model.ToDate)
            {
                ModelState.AddModelError(string.Empty, "From Date cannot be after To Date.");
                return View("Index", model);
            }

            try
            {
                var resultsViewModel = await _newsApiService.SearchAsync(
                    model.Keyword,
                    model.FromDate,
                    model.ToDate);

                return View("Results", resultsViewModel);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("parametersMissing", StringComparison.OrdinalIgnoreCase))
                {
                    model.ErrorMessage = "Please enter a keyword. The NewsAPI everything endpoint does not support a blank search.";
                }
                else if (ex.Message.Contains("apiKeyInvalid", StringComparison.OrdinalIgnoreCase))
                {
                    model.ErrorMessage = "Your NewsAPI key appears to be invalid. Please check your configuration.";
                }
                else if (ex.Message.Contains("apiKeyMissing", StringComparison.OrdinalIgnoreCase))
                {
                    model.ErrorMessage = "Your NewsAPI key is missing from configuration.";
                }
                else if (ex.Message.Contains("rateLimited", StringComparison.OrdinalIgnoreCase))
                {
                    model.ErrorMessage = "The News API rate limit was reached. Please wait and try again.";
                }
                else
                {
                    model.ErrorMessage = "Unable to retrieve news articles right now. Please try again later.";
                }

                return View("Index", model);
            }
        }

        [HttpGet]
        public IActionResult Read(string articleUrl, string articleTitle)
        {
            if (string.IsNullOrWhiteSpace(articleUrl))
            {
                return RedirectToAction("Index");
            }

            _sessionArticleTracker.TrackClick(HttpContext.Session, articleTitle, articleUrl);

            return Redirect(articleUrl);
        }

        [HttpGet]
        public IActionResult Stats()
        {
            var stats = _sessionArticleTracker.GetStats(HttpContext.Session);
            return View(stats);
        }
    }
}