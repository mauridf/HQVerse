using HQVerse.Application.DTOs.Search;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Busca global unificada em todas as entidades
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Busca global: personagens, séries, edições, editoras, equipes, criadores e arcos
    /// </summary>
    /// <param name="query">Termo de busca (mínimo 2 caracteres)</param>
    /// <response code="200">Resultados da busca</response>
    [HttpGet]
    [ProducesResponseType(typeof(SearchResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResultDto>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(new SearchResultDto());

        var result = await _searchService.SearchAsync(query, cancellationToken);
        return Ok(result);
    }
}