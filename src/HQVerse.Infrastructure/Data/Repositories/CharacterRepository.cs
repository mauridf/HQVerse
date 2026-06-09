using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HQVerse.Infrastructure.Data.Repositories;

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(HQVerseDbContext context) : base(context) { }

    public async Task<Character?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ComicVineId == comicVineId, cancellationToken);
    }

    public async Task<IEnumerable<Character>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => EF.Functions.ILike(c.Name, $"%{name}%"))
            .Include(c => c.Publisher)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Character>> GetByPublisherIdAsync(int publisherId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.PublisherId == publisherId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}