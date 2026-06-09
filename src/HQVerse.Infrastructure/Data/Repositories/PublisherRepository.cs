using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HQVerse.Infrastructure.Data.Repositories;

public class PublisherRepository : Repository<Publisher>, IPublisherRepository
{
    public PublisherRepository(HQVerseDbContext context) : base(context) { }

    public async Task<Publisher?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.ComicVineId == comicVineId, cancellationToken);
    }

    public async Task<IEnumerable<Publisher>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => EF.Functions.ILike(p.Name, $"%{name}%"))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}