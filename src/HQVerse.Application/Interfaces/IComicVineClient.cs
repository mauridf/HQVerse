using HQVerse.Application.DTOs.ComicVine;

namespace HQVerse.Application.Interfaces;

public interface IComicVineClient
{
    Task<ComicVineListResponse> SearchAsync(
        string resourceType, string query, int limit = 10, CancellationToken cancellationToken = default);

    Task<ComicVineSingleResponse> GetByIdAsync(
        string resourceType, int comicVineId, CancellationToken cancellationToken = default);
}