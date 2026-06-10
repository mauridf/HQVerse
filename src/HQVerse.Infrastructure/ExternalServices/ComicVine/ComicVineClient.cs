using System.Text.Json;
using HQVerse.Application.DTOs.ComicVine;
using HQVerse.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HQVerse.Infrastructure.ExternalServices.ComicVine;

public class ComicVineClient : IComicVineClient
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    private readonly string _apiKey;
    private readonly ILogger<ComicVineClient> _logger;

    static ComicVineClient()
    {
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "HQVerse/2.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public ComicVineClient(IConfiguration configuration, ILogger<ComicVineClient> logger)
    {
        _apiKey = configuration["ComicVine:ApiKey"]
            ?? throw new InvalidOperationException("ComicVine API Key not configured.");
        _logger = logger;
    }

    public async Task<ComicVineListResponse> SearchAsync(
        string resourceType, string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var url = $"https://comicvine.gamespot.com/api/{resourceType}/?api_key={_apiKey}&format=json&filter=name:{encodedQuery}&limit={limit}";

        _logger.LogInformation("ComicVine Search: {Url}", url);

        return await GetAsync<ComicVineListResponse>(url, cancellationToken);
    }

    public async Task<ComicVineSingleResponse> GetByIdAsync(
        string resourceType, int comicVineId, CancellationToken cancellationToken = default)
    {
        var url = $"https://comicvine.gamespot.com/api/{resourceType}/4000-{comicVineId}/?api_key={_apiKey}&format=json";

        _logger.LogInformation("ComicVine GetById: {Url}", url);

        return await GetAsync<ComicVineSingleResponse>(url, cancellationToken);
    }

    private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("ComicVine error {StatusCode}: {Error}", response.StatusCode, errorBody);
            throw new HttpRequestException($"ComicVine API returned {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<T>(content, options);

        if (result is null)
            throw new InvalidOperationException($"Failed to deserialize ComicVine response.");

        return result;
    }
}