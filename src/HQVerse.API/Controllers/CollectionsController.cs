using System.Security.Claims;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Collections;
using HQVerse.Application.DTOs.ReadingProgress;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Gerencia coleções de HQs dos usuários e progresso de leitura
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CollectionsController : BaseApiController
{
    private readonly ICollectionService _collectionService;
    private readonly ILogger<CollectionsController> _logger;

    public CollectionsController(ICollectionService collectionService, ILogger<CollectionsController> logger)
    {
        _collectionService = collectionService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim is not null ? int.Parse(claim) : 0;
    }

    /// <summary>
    /// Lista coleções de um usuário
    /// </summary>
    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(PaginatedResult<UserCollectionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<UserCollectionDto>>> GetUserCollections(
        int userId,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _collectionService.GetUserCollectionsAsync(userId, paginationParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém detalhes de uma coleção com suas edições
    /// </summary>
    [HttpGet("{collectionId:int}")]
    [ProducesResponseType(typeof(UserCollectionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserCollectionDetailDto>> GetById(
        int collectionId,
        CancellationToken cancellationToken)
    {
        var collection = await _collectionService.GetCollectionByIdAsync(collectionId, cancellationToken);
        return collection is null ? NotFound() : Ok(collection);
    }

    /// <summary>
    /// Cria uma nova coleção
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(UserCollectionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserCollectionDto>> Create(
        [FromBody] CreateCollectionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var collection = await _collectionService.CreateCollectionAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { collectionId = collection.Id }, collection);
    }

    /// <summary>
    /// Atualiza uma coleção
    /// </summary>
    [HttpPut("{collectionId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(UserCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserCollectionDto>> Update(
        int collectionId,
        [FromBody] UpdateCollectionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var collection = await _collectionService.UpdateCollectionAsync(collectionId, userId, dto, cancellationToken);
        return collection is null ? NotFound() : Ok(collection);
    }

    /// <summary>
    /// Remove uma coleção
    /// </summary>
    [HttpDelete("{collectionId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        int collectionId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var deleted = await _collectionService.DeleteCollectionAsync(collectionId, userId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Adiciona uma edição à coleção
    /// </summary>
    [HttpPost("{collectionId:int}/issues")]
    [Authorize]
    [ProducesResponseType(typeof(CollectionIssueDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CollectionIssueDto>> AddIssue(
        int collectionId,
        [FromBody] AddIssueToCollectionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var issue = await _collectionService.AddIssueToCollectionAsync(collectionId, userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { collectionId }, issue);
    }

    /// <summary>
    /// Atualiza status de uma edição na coleção
    /// </summary>
    [HttpPut("{collectionId:int}/issues/{issueId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(CollectionIssueDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CollectionIssueDto>> UpdateIssue(
        int collectionId,
        int issueId,
        [FromBody] UpdateCollectionIssueDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var issue = await _collectionService.UpdateCollectionIssueAsync(collectionId, issueId, userId, dto, cancellationToken);
        return issue is null ? NotFound() : Ok(issue);
    }

    /// <summary>
    /// Remove uma edição da coleção
    /// </summary>
    [HttpDelete("{collectionId:int}/issues/{issueId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveIssue(
        int collectionId,
        int issueId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var removed = await _collectionService.RemoveIssueFromCollectionAsync(collectionId, issueId, userId, cancellationToken);
        return removed ? NoContent() : NotFound();
    }

    /// <summary>
    /// Inicia leitura de uma edição
    /// </summary>
    [HttpPost("reading/start")]
    [Authorize]
    [ProducesResponseType(typeof(ReadingProgressDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ReadingProgressDto>> StartReading(
        [FromBody] StartReadingDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var progress = await _collectionService.StartReadingAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetReadingProgress), new { issueId = dto.IssueId }, progress);
    }

    /// <summary>
    /// Atualiza progresso de leitura
    /// </summary>
    [HttpPut("reading/{issueId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ReadingProgressDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadingProgressDto>> UpdateReadingProgress(
        int issueId,
        [FromBody] UpdateReadingProgressDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var progress = await _collectionService.UpdateReadingProgressAsync(userId, issueId, dto, cancellationToken);
        return progress is null ? NotFound() : Ok(progress);
    }

    /// <summary>
    /// Obtém progresso de leitura de uma edição
    /// </summary>
    [HttpGet("reading/{issueId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ReadingProgressDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadingProgressDto>> GetReadingProgress(
        int issueId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var progress = await _collectionService.GetReadingProgressAsync(userId, issueId, cancellationToken);
        return progress is null ? NotFound() : Ok(progress);
    }

    /// <summary>
    /// Lista edições em leitura ativa
    /// </summary>
    [HttpGet("reading/current")]
    [Authorize]
    [ProducesResponseType(typeof(List<ReadingProgressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReadingProgressDto>>> GetCurrentlyReading(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var progress = await _collectionService.GetCurrentlyReadingAsync(userId, cancellationToken);
        return Ok(progress);
    }
}