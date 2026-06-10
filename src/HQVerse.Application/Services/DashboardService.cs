using HQVerse.Application.DTOs.Dashboard;
using HQVerse.Application.DTOs.Scans;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating dashboard data...");

        // Buscar estatísticas
        var totalIssues = await _unitOfWork.ComicIssues.CountAsync(i => true, cancellationToken);
        var totalPublishers = await _unitOfWork.Publishers.CountAsync(p => true, cancellationToken);
        var totalScans = await _unitOfWork.Scans.CountAsync(s => true, cancellationToken);
        var totalUsers = await _unitOfWork.Users.CountAsync(u => true, cancellationToken);
        var totalReviews = await _unitOfWork.Reviews.CountAsync(r => true, cancellationToken);

        // Buscar últimos scans (5 mais recentes)
        var allScans = await _unitOfWork.Scans.GetAllAsync(cancellationToken);
        var latestScans = allScans
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .Select(s => new ScanDto
            {
                Id = s.Id,
                IssueId = s.IssueId,
                IssueTitle = s.Issue?.Title ?? "",
                SeriesName = s.Issue?.Series?.Name ?? "",
                IssueNumber = s.Issue?.IssueNumber ?? "",
                CoverUrl = s.Issue?.CoverUrl,
                ScanGroupId = s.ScanGroupId,
                ScanGroupName = s.ScanGroup?.Name,
                Version = s.Version,
                Language = s.Language,
                Pages = s.Pages,
                FileSize = s.FileSize,
                Format = s.Format,
                Quality = s.Quality,
                UploaderUserId = s.UploaderUserId,
                CreatedAt = s.CreatedAt
            })
            .ToList();

        // Buscar top editoras (ordenado por quantidade de edições)
        var allPublishers = await _unitOfWork.Publishers.GetAllAsync(cancellationToken);
        var topPublishers = allPublishers
            .Select(p => new PublisherStatsDto
            {
                Id = p.Id,
                Name = p.Name,
                LogoUrl = p.LogoUrl,
                IssueCount = p.ComicSeries.Sum(cs => cs.Issues.Count)
            })
            .OrderByDescending(p => p.IssueCount)
            .Take(6)
            .ToList();

        var dashboard = new DashboardDto
        {
            Stats = new DashboardStatsDto
            {
                TotalIssues = totalIssues,
                TotalPublishers = totalPublishers,
                TotalScans = totalScans,
                TotalUsers = totalUsers,
                TotalReviews = totalReviews
            },
            LatestScans = latestScans,
            TopPublishers = topPublishers
        };

        _logger.LogInformation("Dashboard data generated: {Issues} issues, {Publishers} publishers, {Scans} scans",
            totalIssues, totalPublishers, totalScans);

        return dashboard;
    }
}