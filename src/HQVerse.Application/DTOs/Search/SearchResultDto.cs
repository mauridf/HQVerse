namespace HQVerse.Application.DTOs.Search;

public class SearchResultDto
{
    public List<SearchItemDto> Characters { get; set; } = new();
    public List<SearchItemDto> ComicSeries { get; set; } = new();
    public List<SearchItemDto> ComicIssues { get; set; } = new();
    public List<SearchItemDto> Publishers { get; set; } = new();
    public List<SearchItemDto> Teams { get; set; } = new();
    public List<SearchItemDto> Creators { get; set; } = new();
    public List<SearchItemDto> StoryArcs { get; set; } = new();
}

public class SearchItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Subtitle { get; set; }
}