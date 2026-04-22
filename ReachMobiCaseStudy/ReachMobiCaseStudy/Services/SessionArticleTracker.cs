using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ReachMobiCaseStudy.Models;

namespace ReachMobiCaseStudy.Services;

public class SessionArticleTracker : ISessionArticleTracker
{
    private const string SessionKey = "ArticleClickStats";

    public void TrackClick(ISession session, string? title, string url)
    {
        var stats = GetStats(session).Articles;

        var existing = stats.FirstOrDefault(x => x.Url == url);

        if (existing != null)
        {
            existing.ClickCount++;
        }
        else
        {
            stats.Add(new ArticleClickItemViewModel
            {
                Url = url,
                Title = string.IsNullOrWhiteSpace(title) ? url : title,
                ClickCount = 1
            });
        }

        SaveStats(session, stats);
    }

    public ArticleClickStatsViewModel GetStats(ISession session)
    {
        var stats = GetStoredStats(session)
            .OrderByDescending(a => a.ClickCount)
            .ThenBy(a => a.Title)
            .ToList();

        return new ArticleClickStatsViewModel
        {
            Articles = stats,
            TotalClicks = stats.Sum(a => a.ClickCount)
        };
    }

    private static List<ArticleClickItemViewModel> GetStoredStats(ISession session)
    {
        var json = session.GetString(SessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<ArticleClickItemViewModel>();
        }

        return JsonSerializer.Deserialize<List<ArticleClickItemViewModel>>(json) ?? new List<ArticleClickItemViewModel>();
    }

    private static void SaveStats(ISession session, List<ArticleClickItemViewModel> stats)
    {
        var json = JsonSerializer.Serialize(stats);
        session.SetString(SessionKey, json);
    }
}
