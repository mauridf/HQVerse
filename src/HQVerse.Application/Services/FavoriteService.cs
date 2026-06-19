using AutoMapper;
using HQVerse.Application.DTOs.Favorites;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Enums;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(IUnitOfWork unitOfWork, ILogger<FavoriteService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<FavoriteGroupDto>> GetUserFavoritesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var favorites = await _unitOfWork.Favorites.FindAsync(
            f => f.UserId == userId, cancellationToken);

        var grouped = favorites
            .GroupBy(f => f.EntityType)
            .Select(g => new FavoriteGroupDto
            {
                EntityType = g.Key.ToString(),
                Count = g.Count(),
                Items = g.Select(f => new FavoriteDto
                {
                    UserId = f.UserId,
                    EntityType = f.EntityType,
                    EntityId = f.EntityId
                }).ToList()
            })
            .ToList();

        return grouped;
    }

    public async Task AddFavoriteAsync(int userId, AddFavoriteDto dto, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<EntityType>(dto.EntityType, ignoreCase: true, out var entityType))
            throw new InvalidOperationException($"Invalid entity type: {dto.EntityType}");

        var exists = await _unitOfWork.Favorites.ExistsAsync(
            f => f.UserId == userId && f.EntityType == entityType && f.EntityId == dto.EntityId, cancellationToken);

        if (exists) return;

        var favorite = new UserFavorite
        {
            UserId = userId,
            EntityType = entityType,
            EntityId = dto.EntityId
        };

        await _unitOfWork.Favorites.AddAsync(favorite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Favorite added: User {UserId}, {Type} {EntityId}", userId, entityType, dto.EntityId);
    }

    public async Task RemoveFavoriteAsync(int userId, string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<EntityType>(entityType, ignoreCase: true, out var type))
            return;

        var favorite = await _unitOfWork.Favorites.FindAsync(
            f => f.UserId == userId && f.EntityType == type && f.EntityId == entityId, cancellationToken);

        var fav = favorite.FirstOrDefault();
        if (fav is null) return;

        await _unitOfWork.Favorites.DeleteAsync(fav, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsFavoritedAsync(int userId, string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<EntityType>(entityType, ignoreCase: true, out var type))
            return false;

        return await _unitOfWork.Favorites.ExistsAsync(
            f => f.UserId == userId && f.EntityType == type && f.EntityId == entityId, cancellationToken);
    }
}
