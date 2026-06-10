using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Scans;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Enums;
using HQVerse.Domain.Exceptions;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class ScanService : IScanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ScanService> _logger;

    public ScanService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScanService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // ==================== SCAN GROUPS ====================
    public async Task<List<ScanGroupDto>> GetAllScanGroupsAsync(CancellationToken cancellationToken = default)
    {
        var groups = await _unitOfWork.ScanGroups.GetAllAsync(cancellationToken);
        return groups.Select(g => new ScanGroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            LogoUrl = g.LogoUrl,
            Website = g.Website,
            Discord = g.Discord,
            Telegram = g.Telegram,
            ScanCount = g.Scans.Count
        }).ToList();
    }

    public async Task<ScanGroupDto?> GetScanGroupByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var group = await _unitOfWork.ScanGroups.GetByIdAsync(id, cancellationToken);
        if (group is null) return null;

        return new ScanGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            LogoUrl = group.LogoUrl,
            Website = group.Website,
            Discord = group.Discord,
            Telegram = group.Telegram,
            ScanCount = group.Scans.Count
        };
    }

    public async Task<ScanGroupDto> CreateScanGroupAsync(CreateScanGroupDto dto, CancellationToken cancellationToken = default)
    {
        var group = new ScanGroup
        {
            Name = dto.Name,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            Website = dto.Website,
            Discord = dto.Discord,
            Telegram = dto.Telegram
        };

        await _unitOfWork.ScanGroups.AddAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Scan group created: {Name}", dto.Name);

        return new ScanGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            LogoUrl = group.LogoUrl,
            Website = group.Website,
            Discord = group.Discord,
            Telegram = group.Telegram,
            ScanCount = 0
        };
    }

    // ==================== SCANS ====================
    public async Task<PaginatedResult<ScanDto>> GetScansByIssueIdAsync(
        int issueId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var scans = await _unitOfWork.Scans.FindAsync(
            s => s.IssueId == issueId, cancellationToken);

        var scansList = scans.ToList();
        var paginatedItems = scansList
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        var dtos = paginatedItems.Select(MapScanToDto).ToList();

        return new PaginatedResult<ScanDto>
        {
            Items = dtos,
            TotalCount = scansList.Count,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<ScanDetailDto?> GetScanByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var scan = await _unitOfWork.Scans.GetByIdAsync(id, cancellationToken);
        if (scan is null) return null;

        return new ScanDetailDto
        {
            Id = scan.Id,
            IssueId = scan.IssueId,
            IssueTitle = scan.Issue?.Title ?? "",
            SeriesName = scan.Issue?.Series?.Name ?? "",
            IssueNumber = scan.Issue?.IssueNumber ?? "",
            CoverUrl = scan.Issue?.CoverUrl,
            ScanGroupId = scan.ScanGroupId,
            ScanGroupName = scan.ScanGroup?.Name,
            Version = scan.Version,
            Language = scan.Language,
            Pages = scan.Pages,
            FileSize = scan.FileSize,
            Format = scan.Format,
            Quality = scan.Quality,
            UploaderUserId = scan.UploaderUserId,
            Links = scan.Links.Select(l => new ScanLinkDto
            {
                Id = l.Id,
                Type = l.Type.ToString(),
                Url = l.Url,
                IsOnline = l.IsOnline
            }).ToList(),
            Synopsis = scan.Issue?.Synopsis,
            ISBN = scan.Issue?.ISBN,
            UPC = scan.Issue?.UPC,
            CreatedAt = scan.CreatedAt
        };
    }

    public async Task<ScanDto> CreateScanAsync(int uploaderUserId, CreateScanDto dto, CancellationToken cancellationToken = default)
    {
        // Verificar se a edição existe
        var issue = await _unitOfWork.ComicIssues.GetByIdAsync(dto.IssueId, cancellationToken);
        if (issue is null)
            throw new EntityNotFoundException(nameof(ComicIssue), dto.IssueId);

        var scan = new Scan
        {
            IssueId = dto.IssueId,
            ScanGroupId = dto.ScanGroupId,
            Version = dto.Version,
            Language = dto.Language,
            Pages = dto.Pages,
            FileSize = dto.FileSize,
            Format = dto.Format,
            Quality = dto.Quality,
            UploaderUserId = uploaderUserId,
            CreatedAt = DateTime.UtcNow
        };

        // Adicionar links
        foreach (var linkDto in dto.Links)
        {
            var linkType = linkDto.Type.ToUpper() switch
            {
                "DOWNLOAD" => ScanLinkType.Download,
                "READONLINE" => ScanLinkType.ReadOnline,
                "MIRROR" => ScanLinkType.Mirror,
                "TORRENT" => ScanLinkType.Torrent,
                _ => ScanLinkType.Download
            };

            scan.Links.Add(new ScanLink
            {
                Type = linkType,
                Url = linkDto.Url,
                IsOnline = linkDto.IsOnline
            });
        }

        await _unitOfWork.Scans.AddAsync(scan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Scan created: Issue {IssueId} by User {UserId}", dto.IssueId, uploaderUserId);

        return MapScanToDto(scan);
    }

    public async Task<bool> DeleteScanAsync(int scanId, int userId, CancellationToken cancellationToken = default)
    {
        var scan = await _unitOfWork.Scans.GetByIdAsync(scanId, cancellationToken);
        if (scan is null) return false;

        // Verificar permissão (dono do scan, admin ou moderador)
        if (scan.UploaderUserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own scans.");

        await _unitOfWork.Scans.DeleteAsync(scan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Scan deleted: {ScanId}", scanId);
        return true;
    }

    public async Task<PaginatedResult<ScanDto>> SearchScansAsync(
        string query, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var allScans = await _unitOfWork.Scans.GetAllAsync(cancellationToken);
        var filtered = allScans.Where(s =>
            (s.Issue?.Title != null && s.Issue.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (s.Issue?.Series?.Name != null && s.Issue.Series.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (s.ScanGroup?.Name != null && s.ScanGroup.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        var paginatedItems = filtered
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<ScanDto>
        {
            Items = paginatedItems.Select(MapScanToDto).ToList(),
            TotalCount = filtered.Count,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<PaginatedResult<ScanDto>> GetLatestScansAsync(
        PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var allScans = await _unitOfWork.Scans.GetAllAsync(cancellationToken);
        var orderedScans = allScans
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

        var paginatedItems = orderedScans
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<ScanDto>
        {
            Items = paginatedItems.Select(MapScanToDto).ToList(),
            TotalCount = orderedScans.Count,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    // ==================== HELPER ====================
    private static ScanDto MapScanToDto(Scan scan)
    {
        return new ScanDto
        {
            Id = scan.Id,
            IssueId = scan.IssueId,
            IssueTitle = scan.Issue?.Title ?? "",
            SeriesName = scan.Issue?.Series?.Name ?? "",
            IssueNumber = scan.Issue?.IssueNumber ?? "",
            CoverUrl = scan.Issue?.CoverUrl,
            ScanGroupId = scan.ScanGroupId,
            ScanGroupName = scan.ScanGroup?.Name,
            Version = scan.Version,
            Language = scan.Language,
            Pages = scan.Pages,
            FileSize = scan.FileSize,
            Format = scan.Format,
            Quality = scan.Quality,
            UploaderUserId = scan.UploaderUserId,
            Links = scan.Links.Select(l => new ScanLinkDto
            {
                Id = l.Id,
                Type = l.Type.ToString(),
                Url = l.Url,
                IsOnline = l.IsOnline
            }).ToList(),
            CreatedAt = scan.CreatedAt
        };
    }
}