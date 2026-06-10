using System.Text.Json.Serialization;

namespace HQVerse.Application.DTOs.ComicVine;

// Resposta genérica da API
public class ComicVineResponse<T>
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = "OK";

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("number_of_page_results")]
    public int NumberOfPageResults { get; set; }

    [JsonPropertyName("number_of_total_results")]
    public int NumberOfTotalResults { get; set; }

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("results")]
    public T Results { get; set; } = default!;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}

// Resultado de busca
public class ComicVineSearchResult
{
    [JsonPropertyName("aliases")]
    public string? Aliases { get; set; }

    [JsonPropertyName("api_detail_url")]
    public string ApiDetailUrl { get; set; } = string.Empty;

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("site_detail_url")]
    public string SiteDetailUrl { get; set; } = string.Empty;

    // Publishers
    [JsonPropertyName("location_address")]
    public string? LocationAddress { get; set; }

    [JsonPropertyName("location_city")]
    public string? LocationCity { get; set; }

    [JsonPropertyName("location_state")]
    public string? LocationState { get; set; }

    // Characters
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }

    [JsonPropertyName("birth")]
    public string? Birth { get; set; }

    [JsonPropertyName("gender")]
    public int? Gender { get; set; }

    // Volumes (ComicSeries)
    [JsonPropertyName("start_year")]
    public string? StartYear { get; set; }

    [JsonPropertyName("count_of_issues")]
    public int? CountOfIssues { get; set; }

    [JsonPropertyName("publisher")]
    public ComicVinePublisher? Publisher { get; set; }

    // Issues
    [JsonPropertyName("issue_number")]
    public string? IssueNumber { get; set; }

    [JsonPropertyName("cover_date")]
    public string? CoverDate { get; set; }

    [JsonPropertyName("volume")]
    public ComicVineVolume? Volume { get; set; }
}

public class ComicVineImage
{
    [JsonPropertyName("icon_url")]
    public string IconUrl { get; set; } = string.Empty;

    [JsonPropertyName("medium_url")]
    public string MediumUrl { get; set; } = string.Empty;

    [JsonPropertyName("screen_url")]
    public string ScreenUrl { get; set; } = string.Empty;

    [JsonPropertyName("small_url")]
    public string SmallUrl { get; set; } = string.Empty;

    [JsonPropertyName("super_url")]
    public string SuperUrl { get; set; } = string.Empty;

    [JsonPropertyName("thumb_url")]
    public string ThumbUrl { get; set; } = string.Empty;

    [JsonPropertyName("tiny_url")]
    public string TinyUrl { get; set; } = string.Empty;

    [JsonPropertyName("original_url")]
    public string OriginalUrl { get; set; } = string.Empty;
}

public class ComicVinePublisher
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class ComicVineVolume
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

// Resposta para GetById (results é um objeto único)
public class ComicVineSingleResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = "OK";

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("number_of_page_results")]
    public int NumberOfPageResults { get; set; }

    [JsonPropertyName("number_of_total_results")]
    public int NumberOfTotalResults { get; set; }

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("results")]
    public ComicVineSearchResult Results { get; set; } = new();

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}

// Resposta para Search (results é um array)
public class ComicVineListResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = "OK";

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("number_of_page_results")]
    public int NumberOfPageResults { get; set; }

    [JsonPropertyName("number_of_total_results")]
    public int NumberOfTotalResults { get; set; }

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("results")]
    public List<ComicVineSearchResult> Results { get; set; } = new();

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}