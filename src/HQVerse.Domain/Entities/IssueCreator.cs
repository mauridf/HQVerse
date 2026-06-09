namespace HQVerse.Domain.Entities;

public class IssueCreator
{
    public int IssueId { get; set; }
    public int CreatorId { get; set; }
    public int CreatorRoleId { get; set; }

    // Navigation properties
    public ComicIssue Issue { get; set; } = null!;
    public Creator Creator { get; set; } = null!;
    public CreatorRole CreatorRole { get; set; } = null!;
}