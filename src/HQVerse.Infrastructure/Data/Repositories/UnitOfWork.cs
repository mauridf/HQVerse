using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace HQVerse.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HQVerseDbContext _context;
    private IDbContextTransaction? _transaction;

    public IPublisherRepository Publishers { get; }
    public ICharacterRepository Characters { get; }
    public IComicSeriesRepository ComicSeries { get; }
    public IComicIssueRepository ComicIssues { get; }
    public IUserRepository Users { get; }
    public IReviewRepository Reviews { get; }
    public IRepository<Domain.Entities.Team> Teams { get; }
    public IRepository<Domain.Entities.Creator> Creators { get; }
    public IRepository<Domain.Entities.StoryArc> StoryArcs { get; }
    public IRepository<Domain.Entities.Scan> Scans { get; }
    public IRepository<Domain.Entities.ScanGroup> ScanGroups { get; }
    public IRepository<Domain.Entities.UserCollection> Collections { get; }
    public IRepository<Domain.Entities.Comment> Comments { get; }
    public IRepository<Domain.Entities.ExternalMapping> ExternalMappings { get; }
    public IRepository<Domain.Entities.ReadingProgress> ReadingProgresses { get; }
    public IRepository<Domain.Entities.Universe> Universes { get; }
    public IRepository<Domain.Entities.UserFavorite> Favorites { get; }
    public IRepository<Domain.Entities.ReviewLike> ReviewLikes { get; }
    public IRepository<Domain.Entities.ScanLink> ScanLinks { get; }

    public UnitOfWork(HQVerseDbContext context,
        IPublisherRepository publishers,
        ICharacterRepository characters,
        IComicSeriesRepository comicSeries,
        IComicIssueRepository comicIssues,
        IUserRepository users,
        IReviewRepository reviews)
    {
        _context = context;
        Publishers = publishers;
        Characters = characters;
        ComicSeries = comicSeries;
        ComicIssues = comicIssues;
        Users = users;
        Reviews = reviews;
        Teams = new Repository<Domain.Entities.Team>(context);
        Creators = new Repository<Domain.Entities.Creator>(context);
        StoryArcs = new Repository<Domain.Entities.StoryArc>(context);
        Scans = new Repository<Domain.Entities.Scan>(context);
        ScanGroups = new Repository<Domain.Entities.ScanGroup>(context);
        Collections = new Repository<Domain.Entities.UserCollection>(context);
        Comments = new Repository<Domain.Entities.Comment>(context);
        ExternalMappings = new Repository<Domain.Entities.ExternalMapping>(context);
        ReadingProgresses = new Repository<Domain.Entities.ReadingProgress>(context);
        Universes = new Repository<Domain.Entities.Universe>(context);
        Favorites = new Repository<Domain.Entities.UserFavorite>(context);
        ReviewLikes = new Repository<Domain.Entities.ReviewLike>(context);
        ScanLinks = new Repository<Domain.Entities.ScanLink>(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}