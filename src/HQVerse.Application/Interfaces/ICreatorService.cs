using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Creators;

namespace HQVerse.Application.Interfaces;

public interface ICreatorService
{
    Task<PaginatedResult<CreatorDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<CreatorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CreatorDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
    Task<CreatorDto> CreateAsync(CreateCreatorDto dto, CancellationToken cancellationToken = default);
    Task<CreatorDto?> UpdateAsync(int id, UpdateCreatorDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
