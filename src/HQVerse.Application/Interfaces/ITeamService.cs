using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Teams;

namespace HQVerse.Application.Interfaces;

public interface ITeamService
{
    Task<PaginatedResult<TeamDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<TeamDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TeamDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
    Task<TeamDto> CreateAsync(CreateTeamDto dto, CancellationToken cancellationToken = default);
    Task<TeamDto?> UpdateAsync(int id, UpdateTeamDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
