using HQVerse.Domain.Enums;

namespace HQVerse.Domain.Entities;

public class ScanLink
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public ScanLinkType Type { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsOnline { get; set; } = true;

    // Navigation properties
    public Scan Scan { get; set; } = null!;
}