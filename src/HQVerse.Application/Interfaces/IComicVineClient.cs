using HQVerse.Application.DTOs.ComicVine;

namespace HQVerse.Application.Interfaces;

public interface IComicVineClient
{
    Task<ComicVineResponse<List<ComicVineSearchResult>>> SearchAsync(
        string resourceType, string query, int limit = 10, CancellationToken cancellationToken = default);

    Task<ComicVineResponse<ComicVineSearchResult>> GetByIdAsync(
        string resourceType, int comicVineId, CancellationToken cancellationToken = default);
}