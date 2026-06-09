using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Publishers;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Gerencia editoras de quadrinhos (Marvel, DC, Panini, etc.)
/// </summary>
public class PublishersController : BaseApiController
{
    private readonly IPublisherService _publisherService;
    private readonly ILogger<PublishersController> _logger;

    public PublishersController(IPublisherService publisherService, ILogger<PublishersController> logger)
    {
        _publisherService = publisherService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as editoras com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<PublisherDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<PublisherDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _publisherService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém uma editora pelo ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PublisherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var publisher = await _publisherService.GetByIdAsync(id, cancellationToken);
        return publisher is null ? NotFound() : Ok(publisher);
    }

    /// <summary>
    /// Cria uma nova editora
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PublisherDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublisherDto>> Create(
        [FromBody] CreatePublisherDto dto,
        CancellationToken cancellationToken)
    {
        var publisher = await _publisherService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = publisher.Id }, publisher);
    }

    /// <summary>
    /// Atualiza uma editora existente
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PublisherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherDto>> Update(
        int id,
        [FromBody] UpdatePublisherDto dto,
        CancellationToken cancellationToken)
    {
        var publisher = await _publisherService.UpdateAsync(id, dto, cancellationToken);
        return publisher is null ? NotFound() : Ok(publisher);
    }

    /// <summary>
    /// Remove uma editora
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _publisherService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}