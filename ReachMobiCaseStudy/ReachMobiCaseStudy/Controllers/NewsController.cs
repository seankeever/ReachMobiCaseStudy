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

        [HttpGet]
        public async Task<IActionResult> Search(string? keyword, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var model = new NewsSearchViewModel
                {
                    Keyword = keyword,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                ModelState.AddModelError(string.Empty, "Please enter a keyword. You can optionally add a date range.");
                return View("Index", model);
            }

            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                var model = new NewsSearchViewModel
                {
                    Keyword = keyword,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                ModelState.AddModelError(string.Empty, "From Date cannot be after To Date.");
                return View("Index", model);
            }

            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                10 => 10,
                20 => 20,
                50 => 50,
                _ => 20
            };

            try
            {
                var resultsViewModel = await _newsApiService.SearchAsync(keyword, fromDate, toDate, page, pageSize);
                return View("Results", resultsViewModel);
            }
            catch (Exception ex)
            {
                var model = new NewsSearchViewModel
                {
                    Keyword = keyword,
                    FromDate = fromDate,
                    ToDate = toDate
                };

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
                else if (ex.Message.Contains("requested too many result", StringComparison.OrdinalIgnoreCase))
                {
                    model.ErrorMessage = "You have requested too many results. Developer accounts are limited to a max of 100 results. You are trying to request results 100 to 120. Please upgrade to a paid plan if you need more results.";
                }
                else
                {
                    model.ErrorMessage = "Unable to retrieve news articles right now. Please try again later.";
                }

                return View("Index", model);
            }
        }

        [HttpPost]
        public IActionResult Search(NewsSearchViewModel model)
        {
            return RedirectToAction("Search", new
            {
                keyword = model.Keyword,
                fromDate = model.FromDate?.ToString("yyyy-MM-dd"),
                toDate = model.ToDate?.ToString("yyyy-MM-dd"),
                page = 1,
                pageSize = 20
            });
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