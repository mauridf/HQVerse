namespace HQVerse.Domain.Entities;

public class ExternalMapping
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public int ExternalId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int InternalId { get; set; }
    public DateTime LastSync { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ExternalSource Source { get; set; } = null!;
}