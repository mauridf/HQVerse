using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Publishers;

namespace HQVerse.Application.Interfaces;

public interface IPublisherService
{
    Task<PaginatedResult<PublisherDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<PublisherDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PublisherDto> CreateAsync(CreatePublisherDto dto, CancellationToken cancellationToken = default);
    Task<PublisherDto?> UpdateAsync(int id, UpdatePublisherDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}