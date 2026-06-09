using HQVerse.Domain.Enums;

namespace HQVerse.Domain.Entities;

public class CollectionIssue
{
    public int CollectionId { get; set; }
    public int IssueId { get; set; }
    public ReadStatus ReadStatus { get; set; } = ReadStatus.Wishlist;
    public int? Rating { get; set; }
    public bool Favorite { get; set; } = false;
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public UserCollection Collection { get; set; } = null!;
    public ComicIssue Issue { get; set; } = null!;
}