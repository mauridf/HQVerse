using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.StoryArcs;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

public class StoryArcsController : BaseApiController
{
    private readonly IStoryArcService _storyArcService;
    private readonly ILogger<StoryArcsController> _logger;

    public StoryArcsController(IStoryArcService storyArcService, ILogger<StoryArcsController> logger)
    {
        _storyArcService = storyArcService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<StoryArcDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<StoryArcDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _storyArcService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<StoryArcDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StoryArcDto>>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Enumerable.Empty<StoryArcDto>());
        var arcs = await _storyArcService.SearchByNameAsync(query, cancellationToken);
        return Ok(arcs);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StoryArcDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoryArcDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var arc = await _storyArcService.GetByIdAsync(id, cancellationToken);
        return arc is null ? NotFound() : Ok(arc);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(StoryArcDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoryArcDto>> Create(
        [FromBody] CreateStoryArcDto dto,
        CancellationToken cancellationToken)
    {
        var arc = await _storyArcService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = arc.Id }, arc);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(StoryArcDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoryArcDto>> Update(
        int id,
        [FromBody] UpdateStoryArcDto dto,
        CancellationToken cancellationToken)
    {
        var arc = await _storyArcService.UpdateAsync(id, dto, cancellationToken);
        return arc is null ? NotFound() : Ok(arc);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _storyArcService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{arcId:int}/issues")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddIssue(int arcId, [FromBody] AddIssueToArcDto dto, CancellationToken cancellationToken)
    {
        await _storyArcService.AddIssueToArcAsync(arcId, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{arcId:int}/issues/{issueId:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveIssue(int arcId, int issueId, CancellationToken cancellationToken)
    {
        await _storyArcService.RemoveIssueFromArcAsync(arcId, issueId, cancellationToken);
        return NoContent();
    }
}
