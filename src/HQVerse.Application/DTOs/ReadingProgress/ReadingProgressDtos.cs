namespace HQVerse.Application.DTOs.ReadingProgress;

public class ReadingProgressDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int IssueId { get; set; }
    public string IssueTitle { get; set; } = string.Empty;
    public string SeriesName { get; set; } = string.Empty;
    public string IssueNumber { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public int CurrentPage { get; set; }
    public decimal ProgressPercent { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public bool IsFinished => FinishedAt.HasValue;
}

public class UpdateReadingProgressDto
{
    public int CurrentPage { get; set; }
    public decimal ProgressPercent { get; set; }
    public bool MarkAsFinished { get; set; } = false;
}

public class StartReadingDto
{
    public int IssueId { get; set; }
    public int TotalPages { get; set; }
}