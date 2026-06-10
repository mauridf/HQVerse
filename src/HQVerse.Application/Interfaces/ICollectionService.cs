using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Collections;
using HQVerse.Application.DTOs.ReadingProgress;

namespace HQVerse.Application.Interfaces;

public interface ICollectionService
{
    // Collections
    Task<PaginatedResult<UserCollectionDto>> GetUserCollectionsAsync(int userId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<UserCollectionDetailDto?> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default);
    Task<UserCollectionDto> CreateCollectionAsync(int userId, CreateCollectionDto dto, CancellationToken cancellationToken = default);
    Task<UserCollectionDto?> UpdateCollectionAsync(int collectionId, int userId, UpdateCollectionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCollectionAsync(int collectionId, int userId, CancellationToken cancellationToken = default);

    // Collection Issues
    Task<CollectionIssueDto> AddIssueToCollectionAsync(int collectionId, int userId, AddIssueToCollectionDto dto, CancellationToken cancellationToken = default);
    Task<CollectionIssueDto?> UpdateCollectionIssueAsync(int collectionId, int issueId, int userId, UpdateCollectionIssueDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoveIssueFromCollectionAsync(int collectionId, int issueId, int userId, CancellationToken cancellationToken = default);

    // Reading Progress
    Task<ReadingProgressDto> StartReadingAsync(int userId, StartReadingDto dto, CancellationToken cancellationToken = default);
    Task<ReadingProgressDto?> UpdateReadingProgressAsync(int userId, int issueId, UpdateReadingProgressDto dto, CancellationToken cancellationToken = default);
    Task<ReadingProgressDto?> GetReadingProgressAsync(int userId, int issueId, CancellationToken cancellationToken = default);
    Task<List<ReadingProgressDto>> GetCurrentlyReadingAsync(int userId, CancellationToken cancellationToken = default);
}