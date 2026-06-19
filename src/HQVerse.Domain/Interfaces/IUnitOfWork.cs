using HQVerse.Domain.Entities;

namespace HQVerse.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPublisherRepository Publishers { get; }
    ICharacterRepository Characters { get; }
    IComicSeriesRepository ComicSeries { get; }
    IComicIssueRepository ComicIssues { get; }
    IUserRepository Users { get; }
    IReviewRepository Reviews { get; }
    IRepository<Team> Teams { get; }
    IRepository<Creator> Creators { get; }
    IRepository<StoryArc> StoryArcs { get; }
    IRepository<Scan> Scans { get; }
    IRepository<ScanGroup> ScanGroups { get; }
    IRepository<UserCollection> Collections { get; }
    IRepository<Comment> Comments { get; }
    IRepository<ExternalMapping> ExternalMappings { get; }
    IRepository<ReadingProgress> ReadingProgresses { get; }
    IRepository<Universe> Universes { get; }
    IRepository<UserFavorite> Favorites { get; }
    IRepository<ReviewLike> ReviewLikes { get; }
    IRepository<ScanLink> ScanLinks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}