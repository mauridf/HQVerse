using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HQVerse.Infrastructure.Data.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(HQVerseDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.IssueId == issueId)
            .Include(r => r.User)
            .Include(r => r.Likes)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .Include(r => r.Issue)
            .ThenInclude(i => i.Series)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<double> GetAverageRatingByIssueIdAsync(int issueId, CancellationToken cancellationToken = default)
    {
        var ratings = await _dbSet
            .Where(r => r.IssueId == issueId)
            .Select(r => r.Rating)
            .ToListAsync(cancellationToken);

        return ratings.Any() ? ratings.Average() : 0;
    }
}