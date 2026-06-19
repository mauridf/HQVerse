using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.ComicSeries;

namespace HQVerse.Application.Interfaces;

public interface IComicSeriesService
{
    Task<PaginatedResult<ComicSeriesDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<ComicSeriesDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ComicSeriesDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
    Task<ComicSeriesDto> CreateAsync(CreateComicSeriesDto dto, CancellationToken cancellationToken = default);
    Task<ComicSeriesDto?> UpdateAsync(int id, CreateComicSeriesDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
