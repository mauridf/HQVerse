namespace HQVerse.Domain.Entities;

public class Scan
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public int? ScanGroupId { get; set; }
    public string? Version { get; set; }
    public string Language { get; set; } = "pt-BR";
    public int? Pages { get; set; }
    public long? FileSize { get; set; }
    public string? Format { get; set; }
    public string? Quality { get; set; }
    public int? UploaderUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ComicIssue Issue { get; set; } = null!;
    public ScanGroup? ScanGroup { get; set; }
    public ICollection<ScanLink> Links { get; set; } = new List<ScanLink>();
}