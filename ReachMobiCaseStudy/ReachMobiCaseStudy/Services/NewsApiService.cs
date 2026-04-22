using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using ReachMobiCaseStudy.Models;

namespace ReachMobiCaseStudy.Services;

public class NewsApiService : INewsApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public NewsApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<NewsSearchResultsViewModel> SearchAsync(
        NewsSearchViewModel model,
        int page = 1,
        int pageSize = 20)
    {
        var apiKey = _configuration["NewsApi:ApiKey"];
        var baseUrl = _configuration["NewsApi:BaseUrl"];

        var q = BuildQuery(model);

        var queryParams = new Dictionary<string, string?>
        {
            ["q"] = q,
            ["qInTitle"] = string.IsNullOrWhiteSpace(model.TitleOnly) ? null : model.TitleOnly,
            ["domains"] = string.IsNullOrWhiteSpace(model.Domains) ? null : model.Domains,
            ["excludeDomains"] = string.IsNullOrWhiteSpace(model.ExcludeDomains) ? null : model.ExcludeDomains,
            ["language"] = "en",
            ["sortBy"] = string.IsNullOrWhiteSpace(model.SortBy) ? "publishedAt" : model.SortBy,
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["apiKey"] = apiKey
        };

        if (model.FromDate.HasValue)
        {
            queryParams["from"] = model.FromDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        if (model.ToDate.HasValue)
        {
            queryParams["to"] = model.ToDate.Value.Date.AddDays(1).AddSeconds(-1)
                .ToString("yyyy-MM-ddTHH:mm:ss");
        }

        var requestUrl = QueryHelpers.AddQueryString(baseUrl!, queryParams!);

        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ApplicationException(error);
        }

        var json = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<NewsApiResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var mappedArticles = apiResponse?.Articles?.Select(a => new NewsArticleViewModel
        {
            Title = a.Title ?? "",
            Description = a.Description,
            Url = a.Url ?? "",
            UrlToImage = a.UrlToImage,
            SourceName = a.Source?.Name,
            PublishedAt = a.PublishedAt
        }).ToList() ?? new List<NewsArticleViewModel>();

        return new NewsSearchResultsViewModel
        {
            Keyword = model.Keyword,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            ExactPhrase = model.ExactPhrase,
            ExcludeWords = model.ExcludeWords,
            TitleOnly = model.TitleOnly,
            Domains = model.Domains,
            ExcludeDomains = model.ExcludeDomains,
            SortBy = model.SortBy,
            Articles = mappedArticles,
            Page = page,
            PageSize = pageSize,
            TotalResults = apiResponse == null ? 0 : apiResponse.TotalResults
        };
    }
    private static string? BuildQuery(NewsSearchViewModel model)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(model.Keyword))
            parts.Add(model.Keyword.Trim());

        if (!string.IsNullOrWhiteSpace(model.ExactPhrase))
            parts.Add($"\"{model.ExactPhrase.Trim()}\"");

        if (!string.IsNullOrWhiteSpace(model.ExcludeWords))
        {
            var excluded = model.ExcludeWords
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(w => "-" + w);

            parts.AddRange(excluded);
        }

        return parts.Count == 0 ? null : string.Join(' ', parts);
    }

}
