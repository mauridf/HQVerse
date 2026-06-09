using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.ComicIssues;

namespace HQVerse.Application.Interfaces;

public interface IComicIssueService
{
    Task<PaginatedResult<ComicIssueDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<ComicIssueDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PaginatedResult<ComicIssueDto>> GetBySeriesIdAsync(int seriesId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<ComicIssueDto> CreateAsync(CreateComicIssueDto dto, CancellationToken cancellationToken = default);
}