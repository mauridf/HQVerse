using HQVerse.Application.DTOs.ComicIssues;

namespace HQVerse.Application.DTOs.Collections;

public class UserCollectionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public int IssueCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserCollectionDetailDto : UserCollectionDto
{
    public List<CollectionIssueDto> Issues { get; set; } = new();
}

public class CollectionIssueDto
{
    public int IssueId { get; set; }
    public string IssueTitle { get; set; } = string.Empty;
    public string SeriesName { get; set; } = string.Empty;
    public string IssueNumber { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public string ReadStatus { get; set; } = "WISHLIST";
    public int? Rating { get; set; }
    public bool Favorite { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
}

public class CreateCollectionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
}

public class UpdateCollectionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
}

public class AddIssueToCollectionDto
{
    public int IssueId { get; set; }
    public string ReadStatus { get; set; } = "WISHLIST";
    public int? Rating { get; set; }
    public bool Favorite { get; set; } = false;
    public string? Notes { get; set; }
}

public class UpdateCollectionIssueDto
{
    public string? ReadStatus { get; set; }
    public int? Rating { get; set; }
    public bool? Favorite { get; set; }
    public string? Notes { get; set; }
}