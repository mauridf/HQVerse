using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HQVerse.Infrastructure.Data.Repositories;

public class ComicSeriesRepository : Repository<ComicSeries>, IComicSeriesRepository
{
    public ComicSeriesRepository(HQVerseDbContext context) : base(context) { }

    public async Task<ComicSeries?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(cs => cs.ComicVineId == comicVineId, cancellationToken);
    }

    public async Task<IEnumerable<ComicSeries>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(cs => EF.Functions.ILike(cs.Name, $"%{name}%"))
            .Include(cs => cs.Publisher)
            .OrderBy(cs => cs.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ComicSeries?> GetWithIssuesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cs => cs.Issues.OrderBy(i => i.IssueNumber))
            .Include(cs => cs.Publisher)
            .FirstOrDefaultAsync(cs => cs.Id == id, cancellationToken);
    }
}