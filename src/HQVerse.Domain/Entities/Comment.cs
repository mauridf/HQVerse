namespace HQVerse.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ReviewId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Review Review { get; set; } = null!;
}