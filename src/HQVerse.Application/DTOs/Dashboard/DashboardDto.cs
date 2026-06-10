using HQVerse.Application.DTOs.Scans;
using HQVerse.Application.DTOs.Publishers;

namespace HQVerse.Application.DTOs.Dashboard;

public class DashboardDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public List<ScanDto> LatestScans { get; set; } = new();
    public List<PublisherStatsDto> TopPublishers { get; set; } = new();
}

public class DashboardStatsDto
{
    public int TotalIssues { get; set; }
    public int TotalPublishers { get; set; }
    public int TotalScans { get; set; }
    public int TotalUsers { get; set; }
    public int TotalReviews { get; set; }
}

public class PublisherStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int IssueCount { get; set; }
}