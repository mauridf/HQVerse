namespace HQVerse.Domain.Entities;

public class ComicIssue
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public int SeriesId { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Synopsis { get; set; }
    public DateTime? CoverDate { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int? PageCount { get; set; }
    public string? ISBN { get; set; }
    public string? UPC { get; set; }
    public string? CoverUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ComicSeries Series { get; set; } = null!;
    public ICollection<IssueCharacter> IssueCharacters { get; set; } = new List<IssueCharacter>();
    public ICollection<IssueTeam> IssueTeams { get; set; } = new List<IssueTeam>();
    public ICollection<IssueCreator> IssueCreators { get; set; } = new List<IssueCreator>();
    public ICollection<StoryArcIssue> StoryArcIssues { get; set; } = new List<StoryArcIssue>();
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CollectionIssue> CollectionIssues { get; set; } = new List<CollectionIssue>();
}