using System.Security.Claims;
using HQVerse.Application.DTOs.Favorites;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<FavoritesController> _logger;

    public FavoritesController(IFavoriteService favoriteService, ILogger<FavoritesController> logger)
    {
        _favoriteService = favoriteService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<FavoriteGroupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FavoriteGroupDto>>> GetMyFavorites(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _favoriteService.GetUserFavoritesAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto dto, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _favoriteService.AddFavoriteAsync(userId, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{entityType}/{entityId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFavorite(string entityType, int entityId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _favoriteService.RemoveFavoriteAsync(userId, entityType, entityId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{entityType}/{entityId:int}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> IsFavorited(string entityType, int entityId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _favoriteService.IsFavoritedAsync(userId, entityType, entityId, cancellationToken);
        return Ok(result);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
