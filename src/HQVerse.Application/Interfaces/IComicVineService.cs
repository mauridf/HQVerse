using HQVerse.Application.DTOs.ComicVine;

namespace HQVerse.Application.Interfaces;

public interface IComicVineService
{
    Task<List<ComicVineSearchResult>> SearchAsync(string query, string resourceType, CancellationToken cancellationToken = default);
    Task<ComicVineSearchResult?> GetByIdAsync(string resourceType, int comicVineId, CancellationToken cancellationToken = default);
    Task SyncPublisherAsync(int comicVineId, CancellationToken cancellationToken = default);
    Task SyncCharacterAsync(int comicVineId, CancellationToken cancellationToken = default);
    Task SyncVolumeAsync(int comicVineId, CancellationToken cancellationToken = default);
    Task SyncIssueAsync(int comicVineId, CancellationToken cancellationToken = default);
}