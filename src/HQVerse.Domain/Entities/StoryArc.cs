namespace HQVerse.Domain.Entities;

public class StoryArc
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties
    public Publisher? Publisher { get; set; }
    public ICollection<StoryArcIssue> StoryArcIssues { get; set; } = new List<StoryArcIssue>();
}