using HQVerse.Domain.Enums;

namespace HQVerse.Application.DTOs.Favorites;

public class FavoriteDto
{
    public int UserId { get; set; }
    public EntityType EntityType { get; set; }
    public int EntityId { get; set; }
    public string? EntityImageUrl { get; set; }
}

public class AddFavoriteDto
{
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
}

public class FavoriteGroupDto
{
    public string EntityType { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<FavoriteDto> Items { get; set; } = new();
}
