using HQVerse.Domain.Entities;

namespace HQVerse.Domain.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Task<Character?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Character>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Character>> GetByPublisherIdAsync(int publisherId, CancellationToken cancellationToken = default);
}