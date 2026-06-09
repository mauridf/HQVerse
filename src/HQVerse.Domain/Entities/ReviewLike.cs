namespace HQVerse.Domain.Entities;

public class ReviewLike
{
    public int UserId { get; set; }
    public int ReviewId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Review Review { get; set; } = null!;
}