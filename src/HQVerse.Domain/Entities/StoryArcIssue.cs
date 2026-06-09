namespace HQVerse.Domain.Entities;

public class StoryArcIssue
{
    public int StoryArcId { get; set; }
    public int IssueId { get; set; }
    public int OrderNumber { get; set; }

    // Navigation properties
    public StoryArc StoryArc { get; set; } = null!;
    public ComicIssue Issue { get; set; } = null!;
}