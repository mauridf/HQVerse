using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Characters;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CharacterService> _logger;

    public CharacterService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CharacterService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<CharacterDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var characters = await _unitOfWork.Characters.GetAllAsync(cancellationToken);
        var totalCount = await _unitOfWork.Characters.CountAsync(c => true, cancellationToken);

        var paginatedItems = characters
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<CharacterDto>
        {
            Items = _mapper.Map<IEnumerable<CharacterDto>>(paginatedItems),
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<CharacterDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var character = await _unitOfWork.Characters.GetByIdAsync(id, cancellationToken);
        return character is null ? null : _mapper.Map<CharacterDto>(character);
    }

    public async Task<IEnumerable<CharacterDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default)
    {
        var characters = await _unitOfWork.Characters.SearchByNameAsync(query, cancellationToken);
        return _mapper.Map<IEnumerable<CharacterDto>>(characters);
    }

    public async Task<CharacterDto> CreateAsync(CreateCharacterDto dto, CancellationToken cancellationToken = default)
    {
        var character = _mapper.Map<Character>(dto);
        character.CreatedAt = DateTime.UtcNow;
        character.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Characters.AddAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Character created: {CharacterName} (ID: {CharacterId})", character.Name, character.Id);
        return _mapper.Map<CharacterDto>(character);
    }

    public async Task<CharacterDto?> UpdateAsync(int id, CreateCharacterDto dto, CancellationToken cancellationToken = default)
    {
        var character = await _unitOfWork.Characters.GetByIdAsync(id, cancellationToken);
        if (character is null) return null;

        _mapper.Map(dto, character);
        character.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Characters.UpdateAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CharacterDto>(character);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var character = await _unitOfWork.Characters.GetByIdAsync(id, cancellationToken);
        if (character is null) return false;

        await _unitOfWork.Characters.DeleteAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
