using HQVerse.Application.DTOs.ComicVine;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Integração com Comic Vine para busca e sincronização de metadados
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ComicVineController : ControllerBase
{
    private readonly IComicVineService _comicVineService;
    private readonly ILogger<ComicVineController> _logger;

    public ComicVineController(IComicVineService comicVineService, ILogger<ComicVineController> logger)
    {
        _comicVineService = comicVineService;
        _logger = logger;
    }

    /// <summary>
    /// Busca na Comic Vine por tipo de recurso
    /// </summary>
    /// <param name="query">Termo de busca</param>
    /// <param name="resourceType">Tipo: publishers, characters, volumes, issues, teams, story_arcs, people</param>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<ComicVineSearchResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ComicVineSearchResult>>> Search(
        [FromQuery] string query,
        [FromQuery] string resourceType = "volumes",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(new List<ComicVineSearchResult>());

        var results = await _comicVineService.SearchAsync(query, resourceType, cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// Sincroniza uma editora da Comic Vine para o banco local
    /// </summary>
    [HttpPost("sync/publisher/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SyncPublisher(
        int comicVineId,
        CancellationToken cancellationToken)
    {
        await _comicVineService.SyncPublisherAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Publisher synced successfully" });
    }

    /// <summary>
    /// Sincroniza um personagem da Comic Vine para o banco local
    /// </summary>
    [HttpPost("sync/character/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncCharacter(
        int comicVineId,
        CancellationToken cancellationToken)
    {
        await _comicVineService.SyncCharacterAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Character synced successfully" });
    }

    /// <summary>
    /// Sincroniza uma série (volume) da Comic Vine para o banco local
    /// </summary>
    [HttpPost("sync/volume/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncVolume(
        int comicVineId,
        CancellationToken cancellationToken)
    {
        await _comicVineService.SyncVolumeAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Volume synced successfully" });
    }

    /// <summary>
    /// Sincroniza uma edição da Comic Vine para o banco local
    /// </summary>
    [HttpPost("sync/issue/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncIssue(
        int comicVineId,
        CancellationToken cancellationToken)
    {
        await _comicVineService.SyncIssueAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Issue synced successfully" });
    }
}