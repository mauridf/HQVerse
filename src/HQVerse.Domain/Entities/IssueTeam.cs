namespace HQVerse.Domain.Entities;

public class IssueTeam
{
    public int IssueId { get; set; }
    public int TeamId { get; set; }

    // Navigation properties
    public ComicIssue Issue { get; set; } = null!;
    public Team Team { get; set; } = null!;
}