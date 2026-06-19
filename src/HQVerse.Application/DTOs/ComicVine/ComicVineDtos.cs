using System.Text.Json;
using System.Text.Json.Serialization;

namespace HQVerse.Application.DTOs.ComicVine;

// ==================== RESPOSTAS GENÉRICAS ====================

public class ComicVineSingleResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = "OK";

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("results")]
    public JsonElement Results { get; set; }
}

public class ComicVineListResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = "OK";

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("results")]
    public List<ComicVineSearchResult> Results { get; set; } = new();
}

// ==================== SEARCH (resultados leves) ====================

public class ComicVineSearchResult
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("publisher")]
    public ComicVinePublisherRef? Publisher { get; set; }

    [JsonPropertyName("volume")]
    public ComicVineVolumeRef? Volume { get; set; }

    [JsonPropertyName("start_year")]
    public string? StartYear { get; set; }

    [JsonPropertyName("count_of_issues")]
    public int? CountOfIssues { get; set; }

    [JsonPropertyName("issue_number")]
    public string? IssueNumber { get; set; }

    [JsonPropertyName("cover_date")]
    public string? CoverDate { get; set; }

    [JsonPropertyName("site_detail_url")]
    public string? SiteDetailUrl { get; set; }

    [JsonPropertyName("api_detail_url")]
    public string? ApiDetailUrl { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

// ==================== DETALHES (objetos completos) ====================

public class ComicVinePublisherDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("location_address")]
    public string? LocationAddress { get; set; }

    [JsonPropertyName("location_city")]
    public string? LocationCity { get; set; }

    [JsonPropertyName("location_state")]
    public string? LocationState { get; set; }

    [JsonPropertyName("site_detail_url")]
    public string? SiteDetailUrl { get; set; }

    [JsonPropertyName("aliases")]
    public string? Aliases { get; set; }
}

public class ComicVineCharacterDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("publisher")]
    public ComicVinePublisherRef? Publisher { get; set; }

    [JsonPropertyName("gender")]
    public JsonElement? Gender { get; set; }

    [JsonPropertyName("birth")]
    public string? Birth { get; set; }

    [JsonPropertyName("aliases")]
    public string? Aliases { get; set; }
}

public class ComicVineVolumeDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("publisher")]
    public ComicVinePublisherRef? Publisher { get; set; }

    [JsonPropertyName("start_year")]
    public string? StartYear { get; set; }

    [JsonPropertyName("count_of_issues")]
    public int? CountOfIssues { get; set; }
}

public class ComicVineIssueDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("issue_number")]
    public string? IssueNumber { get; set; }

    [JsonPropertyName("cover_date")]
    public string? CoverDate { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("volume")]
    public ComicVineVolumeRef? Volume { get; set; }
}

public class ComicVineTeamDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("publisher")]
    public ComicVinePublisherRef? Publisher { get; set; }
}

public class ComicVineStoryArcDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("publisher")]
    public ComicVinePublisherRef? Publisher { get; set; }
}

public class ComicVineCreatorDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImage? Image { get; set; }

    [JsonPropertyName("birth")]
    public string? Birth { get; set; }
}

// ==================== TIPOS COMUNS ====================

public class ComicVineImage
{
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("medium_url")]
    public string? MediumUrl { get; set; }

    [JsonPropertyName("screen_url")]
    public string? ScreenUrl { get; set; }

    [JsonPropertyName("small_url")]
    public string? SmallUrl { get; set; }

    [JsonPropertyName("super_url")]
    public string? SuperUrl { get; set; }

    [JsonPropertyName("thumb_url")]
    public string? ThumbUrl { get; set; }

    [JsonPropertyName("tiny_url")]
    public string? TinyUrl { get; set; }

    [JsonPropertyName("original_url")]
    public string? OriginalUrl { get; set; }
}

public class ComicVinePublisherRef
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class ComicVineVolumeRef
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}