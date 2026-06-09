using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Characters;
using HQVerse.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Gerencia personagens de quadrinhos
/// </summary>
public class CharactersController : BaseApiController
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ILogger<CharactersController> _logger;

    public CharactersController(ICharacterRepository characterRepository, ILogger<CharactersController> logger)
    {
        _characterRepository = characterRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os personagens com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> GetAll(CancellationToken cancellationToken)
    {
        var characters = await _characterRepository.GetAllAsync(cancellationToken);
        return Ok(characters);
    }

    /// <summary>
    /// Busca personagens por nome
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Enumerable.Empty<CharacterDto>());

        var characters = await _characterRepository.SearchByNameAsync(query, cancellationToken);
        return Ok(characters);
    }

    /// <summary>
    /// Obtém um personagem pelo ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var character = await _characterRepository.GetByIdAsync(id, cancellationToken);
        return character is null ? NotFound() : Ok(character);
    }
}