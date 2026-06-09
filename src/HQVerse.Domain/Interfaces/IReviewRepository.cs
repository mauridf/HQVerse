using HQVerse.Domain.Entities;

namespace HQVerse.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingByIssueIdAsync(int issueId, CancellationToken cancellationToken = default);
}