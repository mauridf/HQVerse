using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.StoryArcs;

namespace HQVerse.Application.Interfaces;

public interface IStoryArcService
{
    Task<PaginatedResult<StoryArcDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<StoryArcDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StoryArcDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
    Task<StoryArcDto> CreateAsync(CreateStoryArcDto dto, CancellationToken cancellationToken = default);
    Task<StoryArcDto?> UpdateAsync(int id, UpdateStoryArcDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task AddIssueToArcAsync(int arcId, AddIssueToArcDto dto, CancellationToken cancellationToken = default);
    Task RemoveIssueFromArcAsync(int arcId, int issueId, CancellationToken cancellationToken = default);
}
