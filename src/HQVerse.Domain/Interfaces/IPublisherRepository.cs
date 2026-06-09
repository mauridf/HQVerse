using HQVerse.Domain.Entities;

namespace HQVerse.Domain.Interfaces;

public interface IPublisherRepository : IRepository<Publisher>
{
    Task<Publisher?> GetByComicVineIdAsync(int comicVineId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Publisher>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
}