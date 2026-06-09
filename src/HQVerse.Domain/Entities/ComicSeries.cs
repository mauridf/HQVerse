namespace HQVerse.Domain.Entities;

public class ComicSeries
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public int? UniverseId { get; set; }
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public int? TotalIssues { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }

    // Navigation properties
    public Publisher? Publisher { get; set; }
    public Universe? Universe { get; set; }
    public ICollection<ComicIssue> Issues { get; set; } = new List<ComicIssue>();
}