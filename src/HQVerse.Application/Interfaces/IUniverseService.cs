using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Universes;

namespace HQVerse.Application.Interfaces;

public interface IUniverseService
{
    Task<PaginatedResult<UniverseDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<UniverseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UniverseDto> CreateAsync(CreateUniverseDto dto, CancellationToken cancellationToken = default);
    Task<UniverseDto?> UpdateAsync(int id, UpdateUniverseDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
