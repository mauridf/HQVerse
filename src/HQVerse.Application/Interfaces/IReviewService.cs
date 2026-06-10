using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Reviews;

namespace HQVerse.Application.Interfaces;

public interface IReviewService
{
    Task<PaginatedResult<ReviewDto>> GetByIssueIdAsync(int issueId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<PaginatedResult<ReviewDto>> GetByUserIdAsync(int userId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ReviewDto> CreateAsync(int userId, CreateReviewDto dto, CancellationToken cancellationToken = default);
    Task<ReviewDto?> UpdateAsync(int id, int userId, CreateReviewDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);

    // Comments
    Task<CommentDto> AddCommentAsync(int reviewId, int userId, CreateCommentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommentAsync(int commentId, int userId, CancellationToken cancellationToken = default);

    // Likes
    Task<bool> ToggleLikeAsync(int reviewId, int userId, CancellationToken cancellationToken = default);
    Task<int> GetLikeCountAsync(int reviewId, CancellationToken cancellationToken = default);
}