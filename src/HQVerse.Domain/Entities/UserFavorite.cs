using HQVerse.Domain.Enums;

namespace HQVerse.Domain.Entities;

public class UserFavorite
{
    public int UserId { get; set; }
    public EntityType EntityType { get; set; }
    public int EntityId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}