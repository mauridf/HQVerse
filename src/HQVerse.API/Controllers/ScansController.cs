using System.Security.Claims;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Scans;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Gerencia scans digitalizados e grupos de scan
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScansController : BaseApiController
{
    private readonly IScanService _scanService;
    private readonly ILogger<ScansController> _logger;

    public ScansController(IScanService scanService, ILogger<ScansController> logger)
    {
        _scanService = scanService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim is not null ? int.Parse(claim) : 0;
    }

    // ==================== SCAN GROUPS ====================

    /// <summary>
    /// Lista todos os grupos de scan
    /// </summary>
    [HttpGet("groups")]
    [ProducesResponseType(typeof(List<ScanGroupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ScanGroupDto>>> GetAllGroups(CancellationToken cancellationToken)
    {
        var groups = await _scanService.GetAllScanGroupsAsync(cancellationToken);
        return Ok(groups);
    }

    /// <summary>
    /// Obtém detalhes de um grupo de scan
    /// </summary>
    [HttpGet("groups/{id:int}")]
    [ProducesResponseType(typeof(ScanGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScanGroupDto>> GetGroupById(int id, CancellationToken cancellationToken)
    {
        var group = await _scanService.GetScanGroupByIdAsync(id, cancellationToken);
        return group is null ? NotFound() : Ok(group);
    }

    /// <summary>
    /// Cria um novo grupo de scan
    /// </summary>
    [HttpPost("groups")]
    [Authorize]
    [ProducesResponseType(typeof(ScanGroupDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ScanGroupDto>> CreateGroup(
        [FromBody] CreateScanGroupDto dto,
        CancellationToken cancellationToken)
    {
        var group = await _scanService.CreateScanGroupAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, group);
    }

    // ==================== SCANS ====================

    /// <summary>
    /// Lista scans de uma edição específica
    /// </summary>
    [HttpGet("issue/{issueId:int}")]
    [ProducesResponseType(typeof(PaginatedResult<ScanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ScanDto>>> GetByIssue(
        int issueId,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var scans = await _scanService.GetScansByIssueIdAsync(issueId, paginationParams, cancellationToken);
        return Ok(scans);
    }

    /// <summary>
    /// Lista scans mais recentes
    /// </summary>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(PaginatedResult<ScanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ScanDto>>> GetLatest(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var scans = await _scanService.GetLatestScansAsync(paginationParams, cancellationToken);
        return Ok(scans);
    }

    /// <summary>
    /// Busca scans por termo
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<ScanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ScanDto>>> Search(
        [FromQuery] string query,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var scans = await _scanService.SearchScansAsync(query, paginationParams, cancellationToken);
        return Ok(scans);
    }

    /// <summary>
    /// Obtém detalhes de um scan
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ScanDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScanDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var scan = await _scanService.GetScanByIdAsync(id, cancellationToken);
        return scan is null ? NotFound() : Ok(scan);
    }

    /// <summary>
    /// Cria um novo scan (upload)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ScanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScanDto>> Create(
        [FromBody] CreateScanDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var scan = await _scanService.CreateScanAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = scan.Id }, scan);
    }

    /// <summary>
    /// Remove um scan (apenas o uploader)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var deleted = await _scanService.DeleteScanAsync(id, userId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}