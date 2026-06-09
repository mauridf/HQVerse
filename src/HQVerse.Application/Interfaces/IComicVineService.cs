using HQVerse.Application.DTOs.ComicVine;

namespace HQVerse.Application.Interfaces;

public interface IComicVineService
{
    // Busca
    Task<List<ComicVineSearchResult>> SearchAsync(string query, string resourceType, CancellationToken cancellationToken = default);
    Task<ComicVineSearchResult?> GetByIdAsync(string resourceType, int comicVineId, CancellationToken cancellationToken = default);

    // Sync Publishers
    Task SyncPublisherAsync(int comicVineId, CancellationToken cancellationToken = default);

    // Sync Characters
    Task SyncCharacterAsync(int comicVineId, CancellationToken cancellationToken = default);

    // Sync Teams
    Task SyncTeamAsync(int comicVineId, CancellationToken cancellationToken = default);

    // Sync Creators (People)
    Task SyncCreatorAsync(int comicVineId, CancellationToken cancellationToken = default);

    // Sync Volumes (ComicSeries)
    Task SyncVolumeAsync(int comicVineId, CancellationToken cancellationToken = default);

    // Sync Issues
    Task SyncIssueAsync(int comicVineId, CancellationToken cancellationToken = default);

    // Sync StoryArcs
    Task SyncStoryArcAsync(int comicVineId, CancellationToken cancellationToken = default);
}