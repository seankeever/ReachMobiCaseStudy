using Microsoft.AspNetCore.Http;
using ReachMobiCaseStudy.Models;

namespace ReachMobiCaseStudy.Services;

public interface ISessionArticleTracker
{
    void TrackClick(ISession session, string? title, string url);
    ArticleClickStatsViewModel GetStats(ISession session);
}
