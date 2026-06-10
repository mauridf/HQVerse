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

    public ComicVineClient(HttpClient httpClient, IConfiguration configuration, ILogger<ComicVineClient> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ComicVine:ApiKey"]
            ?? throw new InvalidOperationException("ComicVine API Key not configured.");
        _logger = logger;

        // NÃO usar BaseAddress - construir URL completa em cada chamada
    }

    public async Task<ComicVineResponse<List<ComicVineSearchResult>>> SearchAsync(
        string resourceType, string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        // Construir URL COMPLETA (evita problemas com BaseAddress)
        var url = $"https://comicvine.gamespot.com/api/{resourceType}/?api_key={_apiKey}&format=json&filter=name:{Uri.EscapeDataString(query)}&limit={limit}";

        _logger.LogInformation("ComicVine Search URL: {Url}", url);

        return await GetAsync<ComicVineResponse<List<ComicVineSearchResult>>>(url, cancellationToken);
    }

    public async Task<ComicVineResponse<ComicVineSearchResult>> GetByIdAsync(
        string resourceType, int comicVineId, CancellationToken cancellationToken = default)
    {
        // Construir URL COMPLETA
        var url = $"https://comicvine.gamespot.com/api/{resourceType}/4000-{comicVineId}/?api_key={_apiKey}&format=json";

        _logger.LogInformation("ComicVine GetById URL: {Url}", url);

        return await GetAsync<ComicVineResponse<ComicVineSearchResult>>(url, cancellationToken);
    }

    private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "HQVerse/2.0");
        request.Headers.Add("Accept", "application/json");

        _logger.LogInformation("ComicVine API call: {Url}", url);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("ComicVine API error: {StatusCode} - {ErrorBody}",
                response.StatusCode, errorBody);

            throw new HttpRequestException(
                $"ComicVine API returned {response.StatusCode}: {errorBody}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogDebug("ComicVine response (first 200 chars): {Content}",
            content.Length > 200 ? content[..200] : content);

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