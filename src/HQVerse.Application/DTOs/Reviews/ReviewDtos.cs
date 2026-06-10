namespace HQVerse.Application.DTOs.Reviews;

public class ReviewDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    public int IssueId { get; set; }
    public string IssueTitle { get; set; } = string.Empty;
    public string? SeriesName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int Rating { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ReviewDetailDto : ReviewDto
{
    public List<CommentDto> Comments { get; set; } = new();
}

public class CreateReviewDto
{
    public int IssueId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int Rating { get; set; }
}

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
}