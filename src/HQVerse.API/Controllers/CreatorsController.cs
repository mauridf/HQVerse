using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Creators;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

public class CreatorsController : BaseApiController
{
    private readonly ICreatorService _creatorService;
    private readonly ILogger<CreatorsController> _logger;

    public CreatorsController(ICreatorService creatorService, ILogger<CreatorsController> logger)
    {
        _creatorService = creatorService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<CreatorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<CreatorDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _creatorService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CreatorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CreatorDto>>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Enumerable.Empty<CreatorDto>());
        var creators = await _creatorService.SearchByNameAsync(query, cancellationToken);
        return Ok(creators);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CreatorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreatorDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var creator = await _creatorService.GetByIdAsync(id, cancellationToken);
        return creator is null ? NotFound() : Ok(creator);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreatorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatorDto>> Create(
        [FromBody] CreateCreatorDto dto,
        CancellationToken cancellationToken)
    {
        var creator = await _creatorService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = creator.Id }, creator);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(CreatorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreatorDto>> Update(
        int id,
        [FromBody] UpdateCreatorDto dto,
        CancellationToken cancellationToken)
    {
        var creator = await _creatorService.UpdateAsync(id, dto, cancellationToken);
        return creator is null ? NotFound() : Ok(creator);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _creatorService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
