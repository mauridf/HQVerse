namespace HQVerse.Domain.Entities;

public class CharacterTeam
{
    public int CharacterId { get; set; }
    public int TeamId { get; set; }

    // Navigation properties
    public Character Character { get; set; } = null!;
    public Team Team { get; set; } = null!;
}