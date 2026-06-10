using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Reviews;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Exceptions;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<ReviewDto>> GetByIssueIdAsync(
        int issueId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetByIssueIdAsync(issueId, cancellationToken);
        var reviewsList = reviews.ToList();

        var paginatedItems = reviewsList
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<ReviewDto>>(paginatedItems);

        return new PaginatedResult<ReviewDto>
        {
            Items = dtos,
            TotalCount = reviewsList.Count,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<PaginatedResult<ReviewDto>> GetByUserIdAsync(
        int userId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetByUserIdAsync(userId, cancellationToken);
        var reviewsList = reviews.ToList();

        var paginatedItems = reviewsList
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<ReviewDto>>(paginatedItems);

        return new PaginatedResult<ReviewDto>
        {
            Items = dtos,
            TotalCount = reviewsList.Count,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
        return review is null ? null : _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto> CreateAsync(int userId, CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        // Verificar se o usuário já fez review desta edição
        var existingReviews = await _unitOfWork.Reviews.FindAsync(
            r => r.UserId == userId && r.IssueId == dto.IssueId, cancellationToken);

        if (existingReviews.Any())
            throw new BusinessRuleException("You have already reviewed this issue.");

        // Validar rating
        if (dto.Rating < 1 || dto.Rating > 10)
            throw new ValidationException(
                new Dictionary<string, string[]> { { "Rating", new[] { "Rating must be between 1 and 10." } } });

        var review = new Review
        {
            UserId = userId,
            IssueId = dto.IssueId,
            Title = dto.Title,
            Content = dto.Content,
            Rating = dto.Rating,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Review created: {ReviewId} by User {UserId} for Issue {IssueId}",
            review.Id, userId, dto.IssueId);

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto?> UpdateAsync(int id, int userId, CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null) return null;

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own reviews.");

        review.Title = dto.Title;
        review.Content = dto.Content;
        review.Rating = dto.Rating;
        review.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reviews.UpdateAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null) return false;

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own reviews.");

        await _unitOfWork.Reviews.DeleteAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Review deleted: {ReviewId}", id);
        return true;
    }

    // ==================== COMMENTS ====================
    public async Task<CommentDto> AddCommentAsync(int reviewId, int userId, CreateCommentDto dto, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
            throw new EntityNotFoundException(nameof(Review), reviewId);

        var comment = new Comment
        {
            ReviewId = reviewId,
            UserId = userId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Comment added to review {ReviewId} by user {UserId}", reviewId, userId);

        return new CommentDto
        {
            Id = comment.Id,
            UserId = userId,
            ReviewId = reviewId,
            Content = dto.Content,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId, CancellationToken cancellationToken = default)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId, cancellationToken);
        if (comment is null) return false;

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own comments.");

        await _unitOfWork.Comments.DeleteAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // ==================== LIKES ====================
    public async Task<bool> ToggleLikeAsync(int reviewId, int userId, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
            throw new EntityNotFoundException(nameof(Review), reviewId);

        // Verificar se já curtiu
        var existingLikes = await _unitOfWork.Reviews.FindAsync(
            r => r.Id == reviewId, cancellationToken);

        // Buscar like específico
        var likes = await _unitOfWork.Comments.FindAsync(
            c => c.ReviewId == reviewId, cancellationToken);

        // Usar o método correto - verificar se o like existe
        var likeExists = review.Likes.Any(l => l.UserId == userId);

        if (likeExists)
        {
            var like = review.Likes.First(l => l.UserId == userId);
            review.Likes.Remove(like);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Like removed from review {ReviewId} by user {UserId}", reviewId, userId);
            return false; // Unlike
        }
        else
        {
            review.Likes.Add(new ReviewLike { UserId = userId, ReviewId = reviewId });
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Like added to review {ReviewId} by user {UserId}", reviewId, userId);
            return true; // Like
        }
    }

    public async Task<int> GetLikeCountAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId, cancellationToken);
        return review?.Likes.Count ?? 0;
    }
}