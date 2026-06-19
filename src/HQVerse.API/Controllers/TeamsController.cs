using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Teams;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

public class TeamsController : BaseApiController
{
    private readonly ITeamService _teamService;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
    {
        _teamService = teamService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<TeamDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<TeamDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _teamService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<TeamDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TeamDto>>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Enumerable.Empty<TeamDto>());
        var teams = await _teamService.SearchByNameAsync(query, cancellationToken);
        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var team = await _teamService.GetByIdAsync(id, cancellationToken);
        return team is null ? NotFound() : Ok(team);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamDto>> Create(
        [FromBody] CreateTeamDto dto,
        CancellationToken cancellationToken)
    {
        var team = await _teamService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamDto>> Update(
        int id,
        [FromBody] UpdateTeamDto dto,
        CancellationToken cancellationToken)
    {
        var team = await _teamService.UpdateAsync(id, dto, cancellationToken);
        return team is null ? NotFound() : Ok(team);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _teamService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
