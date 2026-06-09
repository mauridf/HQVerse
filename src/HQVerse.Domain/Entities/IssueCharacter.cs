namespace HQVerse.Domain.Entities;

public class IssueCharacter
{
    public int IssueId { get; set; }
    public int CharacterId { get; set; }

    // Navigation properties
    public ComicIssue Issue { get; set; } = null!;
    public Character Character { get; set; } = null!;
}