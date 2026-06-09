namespace HQVerse.Domain.Entities;

public class UserCollection
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<CollectionIssue> CollectionIssues { get; set; } = new List<CollectionIssue>();
}