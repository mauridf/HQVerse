using System.Xml.Linq;

namespace HQVerse.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int IssueId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public ComicIssue Issue { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ReviewLike> Likes { get; set; } = new List<ReviewLike>();
}