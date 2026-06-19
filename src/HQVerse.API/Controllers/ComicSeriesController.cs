using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.ComicSeries;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

public class ComicSeriesController : BaseApiController
{
    private readonly IComicSeriesService _seriesService;
    private readonly ILogger<ComicSeriesController> _logger;

    public ComicSeriesController(IComicSeriesService seriesService, ILogger<ComicSeriesController> logger)
    {
        _seriesService = seriesService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<ComicSeriesDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ComicSeriesDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _seriesService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ComicSeriesDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ComicSeriesDto>>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Enumerable.Empty<ComicSeriesDto>());
        var series = await _seriesService.SearchByNameAsync(query, cancellationToken);
        return Ok(series);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ComicSeriesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComicSeriesDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var series = await _seriesService.GetByIdAsync(id, cancellationToken);
        return series is null ? NotFound() : Ok(series);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ComicSeriesDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ComicSeriesDto>> Create(
        [FromBody] CreateComicSeriesDto dto,
        CancellationToken cancellationToken)
    {
        var series = await _seriesService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = series.Id }, series);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ComicSeriesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComicSeriesDto>> Update(
        int id,
        [FromBody] CreateComicSeriesDto dto,
        CancellationToken cancellationToken)
    {
        var series = await _seriesService.UpdateAsync(id, dto, cancellationToken);
        return series is null ? NotFound() : Ok(series);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _seriesService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
