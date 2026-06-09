using HQVerse.Application.DTOs.Publishers;

namespace HQVerse.Application.DTOs.ComicSeries;

public class ComicSeriesDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public int? TotalIssues { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public PublisherDto? Publisher { get; set; }
}

public class CreateComicSeriesDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public int? UniverseId { get; set; }
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public int? TotalIssues { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
}