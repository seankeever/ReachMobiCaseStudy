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

    public async Task<NewsSearchResultsViewModel> SearchAsync(string? keyword, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
    {
        var apiKey = _configuration["NewsApi:ApiKey"];
        var baseUrl = _configuration["NewsApi:BaseUrl"];

        var queryParams = new Dictionary<string, string?>
        {
            ["q"] = keyword,
            ["language"] = "en",
            ["sortBy"] = "publishedAt",
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["apiKey"] = apiKey
        };

        if (fromDate.HasValue)
        {
            queryParams["from"] = fromDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        if (toDate.HasValue)
        {
            queryParams["to"] = toDate.Value.AddDays(1).AddSeconds(-1)
                .ToString("yyyy-MM-ddTHH:mm:ss");
        }

        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);

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
            Keyword = keyword,
            FromDate = fromDate,
            ToDate = toDate,
            Articles = mappedArticles,
            Page = page,
            PageSize = pageSize,
            TotalResults = apiResponse == null ? 0 : apiResponse.TotalResults
        };
    }
}
