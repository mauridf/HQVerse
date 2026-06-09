using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.ComicIssues;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Gerencia edições de quadrinhos (ex: Batman #15, Amazing Spider-Man #1)
/// </summary>
public class ComicIssuesController : BaseApiController
{
    private readonly IComicIssueService _issueService;
    private readonly ILogger<ComicIssuesController> _logger;

    public ComicIssuesController(IComicIssueService issueService, ILogger<ComicIssuesController> logger)
    {
        _issueService = issueService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as edições com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<ComicIssueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ComicIssueDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _issueService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém detalhes de uma edição específica
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ComicIssueDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComicIssueDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var issue = await _issueService.GetByIdAsync(id, cancellationToken);
        return issue is null ? NotFound() : Ok(issue);
    }

    /// <summary>
    /// Lista edições de uma série específica
    /// </summary>
    [HttpGet("series/{seriesId:int}")]
    [ProducesResponseType(typeof(PaginatedResult<ComicIssueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ComicIssueDto>>> GetBySeries(
        int seriesId,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _issueService.GetBySeriesIdAsync(seriesId, paginationParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova edição
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ComicIssueDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ComicIssueDto>> Create(
        [FromBody] CreateComicIssueDto dto,
        CancellationToken cancellationToken)
    {
        var issue = await _issueService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = issue.Id }, issue);
    }
}