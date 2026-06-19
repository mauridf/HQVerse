using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Universes;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

public class UniversesController : BaseApiController
{
    private readonly IUniverseService _universeService;
    private readonly ILogger<UniversesController> _logger;

    public UniversesController(IUniverseService universeService, ILogger<UniversesController> logger)
    {
        _universeService = universeService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<UniverseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<UniverseDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _universeService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UniverseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UniverseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var universe = await _universeService.GetByIdAsync(id, cancellationToken);
        return universe is null ? NotFound() : Ok(universe);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(UniverseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UniverseDto>> Create(
        [FromBody] CreateUniverseDto dto,
        CancellationToken cancellationToken)
    {
        var universe = await _universeService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = universe.Id }, universe);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(UniverseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UniverseDto>> Update(
        int id,
        [FromBody] UpdateUniverseDto dto,
        CancellationToken cancellationToken)
    {
        var universe = await _universeService.UpdateAsync(id, dto, cancellationToken);
        return universe is null ? NotFound() : Ok(universe);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _universeService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
