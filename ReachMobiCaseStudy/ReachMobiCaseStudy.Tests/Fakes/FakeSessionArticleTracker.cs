using Microsoft.AspNetCore.Http;
using ReachMobiCaseStudy.Models;
using ReachMobiCaseStudy.Services;

namespace ReachMobiCaseStudy.Tests.Fakes;

/// <summary>
/// No-op tracker used so we can construct <c>NewsController</c> without
/// involving a real session. The Search action under test does not touch
/// the tracker, but the controller's constructor still requires one.
/// </summary>
public class FakeSessionArticleTracker : ISessionArticleTracker
{
    public void TrackClick(ISession session, string? title, string url)
    {
        // intentionally empty
    }

    public ArticleClickStatsViewModel GetStats(ISession session)
    {
        return new ArticleClickStatsViewModel();
    }
}
