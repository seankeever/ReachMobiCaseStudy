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

    public async Task<List<NewsArticleViewModel>> SearchAsync(string? keyword, DateTime? date)
    {
        var apiKey = _configuration["NewsApi:ApiKey"];
        var baseUrl = _configuration["NewsApi:BaseUrl"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("NewsApi:ApiKey is missing from configuration.");
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("NewsApi:BaseUrl is missing from configuration.");
        }

        var queryParams = new Dictionary<string, string?>
        {
            ["q"] = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
            ["language"] = "en",
            ["sortBy"] = "publishedAt",
            ["pageSize"] = "20",
            ["apiKey"] = apiKey
        };

        if (date.HasValue)
        {
            var from = date.Value.Date;
            var to = date.Value.Date.AddDays(1).AddSeconds(-1);

            queryParams["from"] = from.ToString("yyyy-MM-ddTHH:mm:ss");
            queryParams["to"] = to.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams!);
        using var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new ApplicationException($"News API request failed. Status: {(int)response.StatusCode}. Details: {errorBody}");
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var apiResponse = await JsonSerializer.DeserializeAsync<NewsApiResponse>(responseStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return apiResponse?.Articles?
            .Where(a => !string.IsNullOrWhiteSpace(a.Url) && !string.IsNullOrWhiteSpace(a.Title))
            .Select(a => new NewsArticleViewModel
            {
                Title = a.Title ?? "Untitled",
                Description = a.Description,
                Url = a.Url ?? string.Empty,
                UrlToImage = a.UrlToImage,
                SourceName = a.Source?.Name,
                PublishedAt = a.PublishedAt,
                Author = a.Author
            })
            .ToList() ?? new List<NewsArticleViewModel>();
    }

    private class NewsApiResponse
    {
        public List<NewsApiArticle>? Articles { get; set; }
    }

    private class NewsApiArticle
    {
        public NewsApiSource? Source { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? UrlToImage { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

    private class NewsApiSource
    {
        public string? Name { get; set; }
    }
}
