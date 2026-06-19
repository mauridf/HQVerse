using HQVerse.Application.DTOs.Publishers;

namespace HQVerse.Application.DTOs.Teams;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public PublisherDto? Publisher { get; set; }
    public int? PublisherId { get; set; }
    public int? UniverseId { get; set; }
}

public class CreateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public int? UniverseId { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public int? UniverseId { get; set; }
    public string? ImageUrl { get; set; }
}
