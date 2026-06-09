namespace HQVerse.Domain.Entities;

public class Publisher
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Country { get; set; }
    public DateTime? FoundationDate { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<ComicSeries> ComicSeries { get; set; } = new List<ComicSeries>();
    public ICollection<StoryArc> StoryArcs { get; set; } = new List<StoryArc>();
}