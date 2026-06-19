using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.ComicIssues;
using HQVerse.Application.DTOs.StoryArcs;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class StoryArcService : IStoryArcService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<StoryArcService> _logger;

    public StoryArcService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<StoryArcService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<StoryArcDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var arcs = await _unitOfWork.StoryArcs.GetAllAsync(cancellationToken);
        var totalCount = arcs.Count();

        var paginatedItems = arcs
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<StoryArcDto>
        {
            Items = paginatedItems.Select(MapToDto),
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<StoryArcDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var arc = await _unitOfWork.StoryArcs.GetByIdAsync(id, cancellationToken);
        if (arc is null) return null;

        return MapToDetailDto(arc);
    }

    public async Task<IEnumerable<StoryArcDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default)
    {
        var arcs = await _unitOfWork.StoryArcs.FindAsync(
            sa => sa.Name.Contains(query, StringComparison.OrdinalIgnoreCase), cancellationToken);
        return arcs.Select(MapToDto);
    }

    public async Task<StoryArcDto> CreateAsync(CreateStoryArcDto dto, CancellationToken cancellationToken = default)
    {
        var arc = new StoryArc
        {
            Name = dto.Name,
            Description = dto.Description,
            PublisherId = dto.PublisherId,
            ImageUrl = dto.ImageUrl
        };

        await _unitOfWork.StoryArcs.AddAsync(arc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("StoryArc created: {Name} (ID: {Id})", arc.Name, arc.Id);
        return MapToDto(arc);
    }

    public async Task<StoryArcDto?> UpdateAsync(int id, UpdateStoryArcDto dto, CancellationToken cancellationToken = default)
    {
        var arc = await _unitOfWork.StoryArcs.GetByIdAsync(id, cancellationToken);
        if (arc is null) return null;

        arc.Name = dto.Name;
        arc.Description = dto.Description;
        arc.PublisherId = dto.PublisherId;
        arc.ImageUrl = dto.ImageUrl;

        await _unitOfWork.StoryArcs.UpdateAsync(arc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(arc);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var arc = await _unitOfWork.StoryArcs.GetByIdAsync(id, cancellationToken);
        if (arc is null) return false;

        await _unitOfWork.StoryArcs.DeleteAsync(arc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task AddIssueToArcAsync(int arcId, AddIssueToArcDto dto, CancellationToken cancellationToken = default)
    {
        var arc = await _unitOfWork.StoryArcs.GetByIdAsync(arcId, cancellationToken);
        if (arc is null) throw new InvalidOperationException($"StoryArc not found: {arcId}");

        var alreadyExists = arc.StoryArcIssues.Any(sai => sai.IssueId == dto.IssueId);
        if (alreadyExists) return;

        arc.StoryArcIssues.Add(new StoryArcIssue
        {
            StoryArcId = arcId,
            IssueId = dto.IssueId,
            OrderNumber = dto.OrderNumber
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Issue {IssueId} added to StoryArc {ArcId}", dto.IssueId, arcId);
    }

    public async Task RemoveIssueFromArcAsync(int arcId, int issueId, CancellationToken cancellationToken = default)
    {
        var arc = await _unitOfWork.StoryArcs.GetByIdAsync(arcId, cancellationToken);
        if (arc is null) return;

        var arcIssue = arc.StoryArcIssues.FirstOrDefault(sai => sai.IssueId == issueId);
        if (arcIssue is null) return;

        arc.StoryArcIssues.Remove(arcIssue);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private StoryArcDto MapToDto(StoryArc arc)
    {
        return new StoryArcDto
        {
            Id = arc.Id,
            Name = arc.Name,
            Description = arc.Description,
            ImageUrl = arc.ImageUrl,
            PublisherId = arc.PublisherId,
            Publisher = arc.Publisher is null ? null : _mapper.Map<DTOs.Publishers.PublisherDto>(arc.Publisher),
            IssueCount = arc.StoryArcIssues.Count
        };
    }

    private StoryArcDetailDto MapToDetailDto(StoryArc arc)
    {
        var detail = MapToDto(arc) as StoryArcDetailDto ?? new StoryArcDetailDto
        {
            Id = arc.Id,
            Name = arc.Name,
            Description = arc.Description,
            ImageUrl = arc.ImageUrl,
            PublisherId = arc.PublisherId,
            Publisher = arc.Publisher is null ? null : _mapper.Map<DTOs.Publishers.PublisherDto>(arc.Publisher),
            IssueCount = arc.StoryArcIssues.Count,
            Issues = arc.StoryArcIssues
                .OrderBy(sai => sai.OrderNumber)
                .Select(sai => _mapper.Map<ComicIssueDto>(sai.Issue))
                .ToList()
        };

        detail.Issues = arc.StoryArcIssues
            .OrderBy(sai => sai.OrderNumber)
            .Select(sai => _mapper.Map<ComicIssueDto>(sai.Issue))
            .ToList();

        return detail;
    }
}
