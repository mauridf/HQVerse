using HQVerse.Domain.Entities;

namespace HQVerse.Domain.Interfaces;

public interface IComicSeriesRepository : IRepository<ComicSeries>
{
    Task<ComicSeries?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ComicSeries>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<ComicSeries?> GetWithIssuesAsync(int id, CancellationToken cancellationToken = default);
}