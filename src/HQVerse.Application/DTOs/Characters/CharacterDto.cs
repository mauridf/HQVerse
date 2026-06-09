using HQVerse.Application.DTOs.Publishers;

namespace HQVerse.Application.DTOs.Characters;

public class CharacterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? Description { get; set; }
    public string? FirstAppearance { get; set; }
    public string? Gender { get; set; }
    public string? Alignment { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public PublisherDto? Publisher { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCharacterDto
{
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
}