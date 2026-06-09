namespace HQVerse.Domain.Entities;

public class ScanGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? Discord { get; set; }
    public string? Telegram { get; set; }

    // Navigation properties
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
}