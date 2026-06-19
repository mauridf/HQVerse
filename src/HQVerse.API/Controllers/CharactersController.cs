using System.Security.Claims;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Characters;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

public class CharactersController : BaseApiController
{
    private readonly ICharacterService _characterService;
    private readonly ILogger<CharactersController> _logger;

    public CharactersController(ICharacterService characterService, ILogger<CharactersController> logger)
    {
        _characterService = characterService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<CharacterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<CharacterDto>>> GetAll(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _characterService.GetAllAsync(paginationParams, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Enumerable.Empty<CharacterDto>());
        var characters = await _characterService.SearchByNameAsync(query, cancellationToken);
        return Ok(characters);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var character = await _characterService.GetByIdAsync(id, cancellationToken);
        return character is null ? NotFound() : Ok(character);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CharacterDto>> Create(
        [FromBody] CreateCharacterDto dto,
        CancellationToken cancellationToken)
    {
        var character = await _characterService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = character.Id }, character);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterDto>> Update(
        int id,
        [FromBody] CreateCharacterDto dto,
        CancellationToken cancellationToken)
    {
        var character = await _characterService.UpdateAsync(id, dto, cancellationToken);
        return character is null ? NotFound() : Ok(character);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _characterService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
