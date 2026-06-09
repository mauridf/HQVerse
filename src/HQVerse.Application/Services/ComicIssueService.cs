using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.ComicIssues;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class ComicIssueService : IComicIssueService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ComicIssueService> _logger;

    public ComicIssueService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ComicIssueService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<ComicIssueDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var issues = await _unitOfWork.ComicIssues.GetAllAsync(cancellationToken);
        var totalCount = await _unitOfWork.ComicIssues.CountAsync(i => true, cancellationToken);

        var paginatedItems = issues
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<ComicIssueDto>>(paginatedItems);

        return new PaginatedResult<ComicIssueDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<ComicIssueDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var issue = await _unitOfWork.ComicIssues.GetByIdAsync(id, cancellationToken);
        if (issue is null) return null;

        var dto = _mapper.Map<ComicIssueDetailDto>(issue);
        dto.AverageRating = await _unitOfWork.Reviews.GetAverageRatingByIssueIdAsync(id, cancellationToken);
        dto.ReviewCount = await _unitOfWork.Reviews.CountAsync(r => r.IssueId == id, cancellationToken);

        return dto;
    }

    public async Task<PaginatedResult<ComicIssueDto>> GetBySeriesIdAsync(int seriesId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var issues = await _unitOfWork.ComicIssues.GetBySeriesIdAsync(seriesId, cancellationToken);
        var issuesList = issues.ToList();

        var paginatedItems = issuesList
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<ComicIssueDto>
        {
            Items = _mapper.Map<IEnumerable<ComicIssueDto>>(paginatedItems),
            TotalCount = issuesList.Count,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<ComicIssueDto> CreateAsync(CreateComicIssueDto dto, CancellationToken cancellationToken = default)
    {
        var issue = _mapper.Map<ComicIssue>(dto);
        issue.CreatedAt = DateTime.UtcNow;
        issue.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.ComicIssues.AddAsync(issue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Comic issue created: {SeriesId}#{IssueNumber}", issue.SeriesId, issue.IssueNumber);
        return _mapper.Map<ComicIssueDto>(issue);
    }
}