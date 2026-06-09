namespace HQVerse.Domain.Entities;

public class Team
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public int? UniverseId { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties
    public Publisher? Publisher { get; set; }
    public Universe? Universe { get; set; }
    public ICollection<CharacterTeam> CharacterTeams { get; set; } = new List<CharacterTeam>();
    public ICollection<IssueTeam> IssueTeams { get; set; } = new List<IssueTeam>();
}