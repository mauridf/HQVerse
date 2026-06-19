using HQVerse.Application.DTOs.Favorites;

namespace HQVerse.Application.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteGroupDto>> GetUserFavoritesAsync(int userId, CancellationToken cancellationToken = default);
    Task AddFavoriteAsync(int userId, AddFavoriteDto dto, CancellationToken cancellationToken = default);
    Task RemoveFavoriteAsync(int userId, string entityType, int entityId, CancellationToken cancellationToken = default);
    Task<bool> IsFavoritedAsync(int userId, string entityType, int entityId, CancellationToken cancellationToken = default);
}
