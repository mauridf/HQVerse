using System.Text.Json;
using System.Text.Json.Serialization;
using HQVerse.Application.DTOs.ComicVine;
using HQVerse.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HQVerse.Infrastructure.ExternalServices.ComicVine;

public class ComicVineClient : IComicVineClient
{
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(30) };
    private readonly string _apiKey;
    private readonly ILogger<ComicVineClient> _logger;

    private static readonly Dictionary<string, string> ResourcePrefixes = new()
    {
        { "publisher", "4010" },
        { "character", "4005" },
        { "team", "4060" },
        { "person", "4040" },
        { "volume", "4050" },
        { "issue", "4000" },
        { "story_arc", "4045" }
    };

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

        var json = await GetJsonAsync(url, cancellationToken);
        return JsonSerializer.Deserialize<ComicVineListResponse>(json, JsonOptions)!;
    }

    public async Task<ComicVineSingleResponse> GetByIdAsync(
    string resourceType, int comicVineId, CancellationToken cancellationToken = default)
    {
        // Obter o prefixo correto (ex: publisher → 4010)
        var prefix = ResourcePrefixes.GetValueOrDefault(resourceType.ToLower(), "4000");
        var url = $"https://comicvine.gamespot.com/api/{resourceType}/{prefix}-{comicVineId}/?api_key={_apiKey}&format=json";

        _logger.LogInformation("ComicVine GetById: {Url}", url);

        var json = await GetJsonAsync(url, cancellationToken);
        return JsonSerializer.Deserialize<ComicVineSingleResponse>(json, JsonOptions)!;
    }

    /// <summary>
    /// Busca detalhes e desserializa no tipo correto
    /// </summary>
    public async Task<T?> GetDetailAsync<T>(string resourceType, int comicVineId, CancellationToken cancellationToken = default) where T : class
    {
        var response = await GetByIdAsync(resourceType, comicVineId, cancellationToken);

        if (response.Error != "OK" || response.Results.ValueKind != JsonValueKind.Object)
            return null;

        return JsonSerializer.Deserialize<T>(response.Results.GetRawText(), JsonOptions);
    }

    private async Task<string> GetJsonAsync(string url, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogInformation("ComicVine response (first 300 chars): {Json}",
            content.Length > 300 ? content[..300] : content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("ComicVine HTTP {StatusCode}: {Body}", response.StatusCode, content);
            throw new HttpRequestException($"ComicVine API returned {response.StatusCode}");
        }

        return content;
    }

    private static JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
}