using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Characters;

namespace HQVerse.Application.Interfaces;

public interface ICharacterService
{
    Task<PaginatedResult<CharacterDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<CharacterDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CharacterDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
    Task<CharacterDto> CreateAsync(CreateCharacterDto dto, CancellationToken cancellationToken = default);
    Task<CharacterDto?> UpdateAsync(int id, CreateCharacterDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
