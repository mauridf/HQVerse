using System.Text.Json;
using HQVerse.Application.DTOs.ComicVine;
using HQVerse.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HQVerse.Infrastructure.ExternalServices.ComicVine;

public class ComicVineClient : IComicVineClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<ComicVineClient> _logger;
    private const string BaseUrl = "https://comicvine.gamespot.com/api";

    public ComicVineClient(HttpClient httpClient, IConfiguration configuration, ILogger<ComicVineClient> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ComicVine:ApiKey"] ?? throw new InvalidOperationException("ComicVine API Key not configured.");
        _logger = logger;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<ComicVineResponse<List<ComicVineSearchResult>>> SearchAsync(
        string resourceType, string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        var url = $"/{resourceType}/?api_key={_apiKey}&format=json&filter=name:{Uri.EscapeDataString(query)}&limit={limit}";
        return await GetAsync<ComicVineResponse<List<ComicVineSearchResult>>>(url, cancellationToken);
    }

    public async Task<ComicVineResponse<ComicVineSearchResult>> GetByIdAsync(
        string resourceType, int comicVineId, CancellationToken cancellationToken = default)
    {
        var url = $"/{resourceType}/4000-{comicVineId}/?api_key={_apiKey}&format=json";
        return await GetAsync<ComicVineResponse<ComicVineSearchResult>>(url, cancellationToken);
    }

    private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ComicVine API call: {Url}", url);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<T>(content, options);

        if (result is null)
            throw new InvalidOperationException($"Failed to deserialize ComicVine response from {url}");

        return result;
    }
}