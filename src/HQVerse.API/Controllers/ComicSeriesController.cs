using HQVerse.Application.DTOs.ComicSeries;
using HQVerse.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Gerencia séries de quadrinhos (ex: Batman 2016, Amazing Spider-Man 2022)
/// </summary>
public class ComicSeriesController : BaseApiController
{
    private readonly IComicSeriesRepository _seriesRepository;
    private readonly ILogger<ComicSeriesController> _logger;

    public ComicSeriesController(IComicSeriesRepository seriesRepository, ILogger<ComicSeriesController> logger)
    {
        _seriesRepository = seriesRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as séries
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ComicSeriesDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ComicSeriesDto>>> GetAll(CancellationToken cancellationToken)
    {
        var series = await _seriesRepository.GetAllAsync(cancellationToken);
        return Ok(series);
    }

    /// <summary>
    /// Busca séries por nome
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ComicSeriesDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ComicSeriesDto>>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Enumerable.Empty<ComicSeriesDto>());

        var series = await _seriesRepository.SearchByNameAsync(query, cancellationToken);
        return Ok(series);
    }

    /// <summary>
    /// Obtém uma série com suas edições
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ComicSeriesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComicSeriesDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var series = await _seriesRepository.GetWithIssuesAsync(id, cancellationToken);
        return series is null ? NotFound() : Ok(series);
    }
}