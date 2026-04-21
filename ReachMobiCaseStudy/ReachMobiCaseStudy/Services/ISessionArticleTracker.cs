using Microsoft.AspNetCore.Http;
using ReachMobiCaseStudy.Models;

namespace ReachMobiCaseStudy.Services;

public interface ISessionArticleTracker
{
    void TrackClick(ISession session, string url, string? title);
    ArticleClickStatsViewModel GetStats(ISession session);
}
