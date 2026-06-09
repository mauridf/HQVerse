namespace HQVerse.Domain.Entities;

public class Character
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? Description { get; set; }
    public string? FirstAppearance { get; set; }
    public string? Gender { get; set; }
    public string? Alignment { get; set; }
    public int? PublisherId { get; set; }
    public int? UniverseId { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Publisher? Publisher { get; set; }
    public Universe? Universe { get; set; }
    public ICollection<CharacterTeam> CharacterTeams { get; set; } = new List<CharacterTeam>();
    public ICollection<IssueCharacter> IssueCharacters { get; set; } = new List<IssueCharacter>();
}