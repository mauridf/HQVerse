using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HQVerse.Infrastructure.Data.Repositories;

public class ComicIssueRepository : Repository<ComicIssue>, IComicIssueRepository
{
    public ComicIssueRepository(HQVerseDbContext context) : base(context) { }

    public async Task<ComicIssue?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(ci => ci.ComicVineId == comicVineId, cancellationToken);
    }

    public async Task<IEnumerable<ComicIssue>> GetBySeriesIdAsync(int seriesId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ci => ci.SeriesId == seriesId)
            .Include(ci => ci.Series)
            .OrderBy(ci => ci.IssueNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<ComicIssue?> GetBySeriesAndNumberAsync(int seriesId, string issueNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ci => ci.SeriesId == seriesId && ci.IssueNumber == issueNumber, cancellationToken);
    }

    public async Task<IEnumerable<ComicIssue>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ci => EF.Functions.ILike(ci.Title ?? "", $"%{query}%") ||
                         EF.Functions.ILike(ci.Series.Name, $"%{query}%") ||
                         EF.Functions.ILike(ci.IssueNumber, $"%{query}%"))
            .Include(ci => ci.Series)
            .ThenInclude(cs => cs.Publisher)
            .OrderByDescending(ci => ci.CoverDate)
            .Take(50)
            .ToListAsync(cancellationToken);
    }
}