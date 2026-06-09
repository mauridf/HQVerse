namespace HQVerse.Domain.Entities;

public class ReadingProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int IssueId { get; set; }
    public int CurrentPage { get; set; } = 0;
    public decimal ProgressPercent { get; set; } = 0;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ComicIssue Issue { get; set; } = null!;
}