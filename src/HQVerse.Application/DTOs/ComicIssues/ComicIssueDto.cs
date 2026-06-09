namespace HQVerse.Application.DTOs.ComicIssues;

public class ComicIssueDto
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public string SeriesName { get; set; } = string.Empty;
    public string IssueNumber { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Synopsis { get; set; }
    public DateTime? CoverDate { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int? PageCount { get; set; }
    public string? CoverUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ComicIssueDetailDto : ComicIssueDto
{
    public string? ISBN { get; set; }
    public string? UPC { get; set; }
    public List<string> Characters { get; set; } = new();
    public List<string> Teams { get; set; } = new();
    public List<IssueCreatorDto> Creators { get; set; } = new();
}

public class IssueCreatorDto
{
    public string CreatorName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CreateComicIssueDto
{
    public int SeriesId { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Synopsis { get; set; }
    public DateTime? CoverDate { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int? PageCount { get; set; }
    public string? ISBN { get; set; }
    public string? UPC { get; set; }
    public string? CoverUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
}