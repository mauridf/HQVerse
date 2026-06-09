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
    /// <param name="resourceType">
    /// Tipo de recurso: publishers, characters, teams, people, volumes, issues, story_arcs
    /// </param>
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
    /// Sincroniza uma editora da Comic Vine
    /// </summary>
    [HttpPost("sync/publisher/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncPublisher(int comicVineId, CancellationToken cancellationToken)
    {
        await _comicVineService.SyncPublisherAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Publisher synced successfully" });
    }

    /// <summary>
    /// Sincroniza um personagem da Comic Vine
    /// </summary>
    [HttpPost("sync/character/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncCharacter(int comicVineId, CancellationToken cancellationToken)
    {
        await _comicVineService.SyncCharacterAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Character synced successfully" });
    }

    /// <summary>
    /// Sincroniza uma equipe da Comic Vine (Liga da Justiça, Vingadores, X-Men)
    /// </summary>
    [HttpPost("sync/team/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncTeam(int comicVineId, CancellationToken cancellationToken)
    {
        await _comicVineService.SyncTeamAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Team synced successfully" });
    }

    /// <summary>
    /// Sincroniza um criador da Comic Vine (Stan Lee, Jack Kirby, etc.)
    /// </summary>
    [HttpPost("sync/creator/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncCreator(int comicVineId, CancellationToken cancellationToken)
    {
        await _comicVineService.SyncCreatorAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Creator synced successfully" });
    }

    /// <summary>
    /// Sincroniza uma série (volume) da Comic Vine
    /// </summary>
    [HttpPost("sync/volume/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncVolume(int comicVineId, CancellationToken cancellationToken)
    {
        await _comicVineService.SyncVolumeAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Volume synced successfully" });
    }

    /// <summary>
    /// Sincroniza uma edição da Comic Vine
    /// </summary>
    [HttpPost("sync/issue/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncIssue(int comicVineId, CancellationToken cancellationToken)
    {
        await _comicVineService.SyncIssueAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "Issue synced successfully" });
    }

    /// <summary>
    /// Sincroniza um arco de história da Comic Vine (Guerra Civil, Crise Infinita)
    /// </summary>
    [HttpPost("sync/storyarc/{comicVineId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncStoryArc(int comicVineId, CancellationToken cancellationToken)
    {
        await _comicVineService.SyncStoryArcAsync(comicVineId, cancellationToken);
        return Ok(new { Message = "StoryArc synced successfully" });
    }
}