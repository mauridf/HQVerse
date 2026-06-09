using HQVerse.Domain.Entities;

namespace HQVerse.Domain.Interfaces;

public interface IComicIssueRepository : IRepository<ComicIssue>
{
    Task<ComicIssue?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ComicIssue>> GetBySeriesIdAsync(int seriesId, CancellationToken cancellationToken = default);
    Task<ComicIssue?> GetBySeriesAndNumberAsync(int seriesId, string issueNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<ComicIssue>> SearchAsync(string query, CancellationToken cancellationToken = default);
}