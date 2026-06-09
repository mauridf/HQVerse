namespace HQVerse.Domain.Entities;

public class Universe
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }

    // Navigation properties
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<ComicSeries> ComicSeries { get; set; } = new List<ComicSeries>();
}