using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Collections;
using HQVerse.Application.DTOs.ReadingProgress;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Enums;
using HQVerse.Domain.Exceptions;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CollectionService> _logger;

    public CollectionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CollectionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // ==================== COLLECTIONS ====================
    public async Task<PaginatedResult<UserCollectionDto>> GetUserCollectionsAsync(
        int userId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var collections = await _unitOfWork.Collections.FindAsync(
            c => c.UserId == userId, cancellationToken);

        var collectionsList = collections.ToList();
        var paginatedItems = collectionsList
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        var dtos = paginatedItems.Select(c => new UserCollectionDto
        {
            Id = c.Id,
            UserId = c.UserId,
            Username = c.User?.Username ?? "Unknown",
            Name = c.Name,
            Description = c.Description,
            IsPublic = c.IsPublic,
            IssueCount = c.CollectionIssues.Count,
            CreatedAt = c.CreatedAt
        }).ToList();

        return new PaginatedResult<UserCollectionDto>
        {
            Items = dtos,
            TotalCount = collectionsList.Count,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<UserCollectionDetailDto?> GetCollectionByIdAsync(
        int collectionId, CancellationToken cancellationToken = default)
    {
        var collection = await _unitOfWork.Collections.GetByIdAsync(collectionId, cancellationToken);
        if (collection is null) return null;

        var dto = new UserCollectionDetailDto
        {
            Id = collection.Id,
            UserId = collection.UserId,
            Username = collection.User?.Username ?? "Unknown",
            Name = collection.Name,
            Description = collection.Description,
            IsPublic = collection.IsPublic,
            IssueCount = collection.CollectionIssues.Count,
            CreatedAt = collection.CreatedAt,
            Issues = collection.CollectionIssues.Select(ci => new CollectionIssueDto
            {
                IssueId = ci.IssueId,
                IssueTitle = ci.Issue?.Title ?? "",
                SeriesName = ci.Issue?.Series?.Name ?? "",
                IssueNumber = ci.Issue?.IssueNumber ?? "",
                CoverUrl = ci.Issue?.CoverUrl,
                ReadStatus = ci.ReadStatus.ToString().ToUpper(),
                Rating = ci.Rating,
                Favorite = ci.Favorite,
                Notes = ci.Notes,
                AddedAt = ci.AddedAt
            }).ToList()
        };

        return dto;
    }

    public async Task<UserCollectionDto> CreateCollectionAsync(
        int userId, CreateCollectionDto dto, CancellationToken cancellationToken = default)
    {
        var collection = new UserCollection
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            IsPublic = dto.IsPublic,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Collections.AddAsync(collection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Collection created: {Name} by User {UserId}", dto.Name, userId);

        return new UserCollectionDto
        {
            Id = collection.Id,
            UserId = userId,
            Name = collection.Name,
            Description = collection.Description,
            IsPublic = collection.IsPublic,
            IssueCount = 0,
            CreatedAt = collection.CreatedAt
        };
    }

    public async Task<UserCollectionDto?> UpdateCollectionAsync(
        int collectionId, int userId, UpdateCollectionDto dto, CancellationToken cancellationToken = default)
    {
        var collection = await _unitOfWork.Collections.GetByIdAsync(collectionId, cancellationToken);
        if (collection is null) return null;

        if (collection.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own collections.");

        collection.Name = dto.Name;
        collection.Description = dto.Description;
        collection.IsPublic = dto.IsPublic;

        await _unitOfWork.Collections.UpdateAsync(collection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserCollectionDto
        {
            Id = collection.Id,
            UserId = collection.UserId,
            Name = collection.Name,
            Description = collection.Description,
            IsPublic = collection.IsPublic,
            IssueCount = collection.CollectionIssues.Count,
            CreatedAt = collection.CreatedAt
        };
    }

    public async Task<bool> DeleteCollectionAsync(
        int collectionId, int userId, CancellationToken cancellationToken = default)
    {
        var collection = await _unitOfWork.Collections.GetByIdAsync(collectionId, cancellationToken);
        if (collection is null) return false;

        if (collection.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own collections.");

        await _unitOfWork.Collections.DeleteAsync(collection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Collection deleted: {CollectionId}", collectionId);
        return true;
    }

    // ==================== COLLECTION ISSUES ====================
    public async Task<CollectionIssueDto> AddIssueToCollectionAsync(
        int collectionId, int userId, AddIssueToCollectionDto dto, CancellationToken cancellationToken = default)
    {
        var collection = await _unitOfWork.Collections.GetByIdAsync(collectionId, cancellationToken);
        if (collection is null)
            throw new EntityNotFoundException(nameof(UserCollection), collectionId);

        if (collection.UserId != userId)
            throw new UnauthorizedAccessException("You can only add to your own collections.");

        // Verificar se a edição existe
        var issue = await _unitOfWork.ComicIssues.GetByIdAsync(dto.IssueId, cancellationToken);
        if (issue is null)
            throw new EntityNotFoundException(nameof(ComicIssue), dto.IssueId);

        // Verificar se já existe na coleção
        if (collection.CollectionIssues.Any(ci => ci.IssueId == dto.IssueId))
            throw new BusinessRuleException("This issue is already in the collection.");

        // Converter status
        var readStatus = dto.ReadStatus.ToUpper() switch
        {
            "WISHLIST" => ReadStatus.Wishlist,
            "READING" => ReadStatus.Reading,
            "READ" => ReadStatus.Read,
            "ABANDONED" => ReadStatus.Abandoned,
            _ => ReadStatus.Wishlist
        };

        var collectionIssue = new CollectionIssue
        {
            CollectionId = collectionId,
            IssueId = dto.IssueId,
            ReadStatus = readStatus,
            Rating = dto.Rating,
            Favorite = dto.Favorite,
            Notes = dto.Notes,
            AddedAt = DateTime.UtcNow
        };

        collection.CollectionIssues.Add(collectionIssue);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Issue {IssueId} added to collection {CollectionId}", dto.IssueId, collectionId);

        return new CollectionIssueDto
        {
            IssueId = dto.IssueId,
            IssueTitle = issue.Title ?? "",
            SeriesName = issue.Series?.Name ?? "",
            IssueNumber = issue.IssueNumber,
            CoverUrl = issue.CoverUrl,
            ReadStatus = readStatus.ToString().ToUpper(),
            Rating = dto.Rating,
            Favorite = dto.Favorite,
            Notes = dto.Notes,
            AddedAt = DateTime.UtcNow
        };
    }

    public async Task<CollectionIssueDto?> UpdateCollectionIssueAsync(
        int collectionId, int issueId, int userId, UpdateCollectionIssueDto dto, CancellationToken cancellationToken = default)
    {
        var collection = await _unitOfWork.Collections.GetByIdAsync(collectionId, cancellationToken);
        if (collection is null) return null;

        if (collection.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own collections.");

        var collectionIssue = collection.CollectionIssues.FirstOrDefault(ci => ci.IssueId == issueId);
        if (collectionIssue is null) return null;

        if (dto.ReadStatus is not null)
        {
            collectionIssue.ReadStatus = dto.ReadStatus.ToUpper() switch
            {
                "WISHLIST" => ReadStatus.Wishlist,
                "READING" => ReadStatus.Reading,
                "READ" => ReadStatus.Read,
                "ABANDONED" => ReadStatus.Abandoned,
                _ => collectionIssue.ReadStatus
            };
        }

        if (dto.Rating.HasValue)
            collectionIssue.Rating = dto.Rating.Value;

        if (dto.Favorite.HasValue)
            collectionIssue.Favorite = dto.Favorite.Value;

        if (dto.Notes is not null)
            collectionIssue.Notes = dto.Notes;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CollectionIssueDto
        {
            IssueId = collectionIssue.IssueId,
            ReadStatus = collectionIssue.ReadStatus.ToString().ToUpper(),
            Rating = collectionIssue.Rating,
            Favorite = collectionIssue.Favorite,
            Notes = collectionIssue.Notes,
            AddedAt = collectionIssue.AddedAt
        };
    }

    public async Task<bool> RemoveIssueFromCollectionAsync(
        int collectionId, int issueId, int userId, CancellationToken cancellationToken = default)
    {
        var collection = await _unitOfWork.Collections.GetByIdAsync(collectionId, cancellationToken);
        if (collection is null) return false;

        if (collection.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own collections.");

        var collectionIssue = collection.CollectionIssues.FirstOrDefault(ci => ci.IssueId == issueId);
        if (collectionIssue is null) return false;

        collection.CollectionIssues.Remove(collectionIssue);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // ==================== READING PROGRESS ====================
    public async Task<ReadingProgressDto> StartReadingAsync(
        int userId, StartReadingDto dto, CancellationToken cancellationToken = default)
    {
        // Verificar se já existe progresso
        var existingProgress = await _unitOfWork.ReadingProgresses.FindAsync(
            rp => rp.UserId == userId && rp.IssueId == dto.IssueId, cancellationToken);

        if (existingProgress.Any())
            throw new BusinessRuleException("Reading progress already exists for this issue. Use update instead.");

        var progress = new ReadingProgress
        {
            UserId = userId,
            IssueId = dto.IssueId,
            CurrentPage = 0,
            ProgressPercent = 0,
            StartedAt = DateTime.UtcNow
        };

        await _unitOfWork.ReadingProgresses.AddAsync(progress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Reading started: Issue {IssueId} by User {UserId}", dto.IssueId, userId);

        return new ReadingProgressDto
        {
            Id = progress.Id,
            UserId = userId,
            IssueId = dto.IssueId,
            CurrentPage = 0,
            ProgressPercent = 0,
            StartedAt = progress.StartedAt
        };
    }

    public async Task<ReadingProgressDto?> UpdateReadingProgressAsync(
        int userId, int issueId, UpdateReadingProgressDto dto, CancellationToken cancellationToken = default)
    {
        var progressList = await _unitOfWork.ReadingProgresses.FindAsync(
            rp => rp.UserId == userId && rp.IssueId == issueId, cancellationToken);

        var progress = progressList.FirstOrDefault();
        if (progress is null) return null;

        progress.CurrentPage = dto.CurrentPage;
        progress.ProgressPercent = dto.ProgressPercent;

        if (dto.MarkAsFinished)
        {
            progress.FinishedAt = DateTime.UtcNow;
            progress.ProgressPercent = 100;
        }

        await _unitOfWork.ReadingProgresses.UpdateAsync(progress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ReadingProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            IssueId = progress.IssueId,
            CurrentPage = progress.CurrentPage,
            ProgressPercent = progress.ProgressPercent,
            StartedAt = progress.StartedAt,
            FinishedAt = progress.FinishedAt
        };
    }

    public async Task<ReadingProgressDto?> GetReadingProgressAsync(
        int userId, int issueId, CancellationToken cancellationToken = default)
    {
        var progressList = await _unitOfWork.ReadingProgresses.FindAsync(
            rp => rp.UserId == userId && rp.IssueId == issueId, cancellationToken);

        var progress = progressList.FirstOrDefault();
        if (progress is null) return null;

        return new ReadingProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            IssueId = progress.IssueId,
            IssueTitle = progress.Issue?.Title ?? "",
            SeriesName = progress.Issue?.Series?.Name ?? "",
            IssueNumber = progress.Issue?.IssueNumber ?? "",
            CoverUrl = progress.Issue?.CoverUrl,
            CurrentPage = progress.CurrentPage,
            ProgressPercent = progress.ProgressPercent,
            StartedAt = progress.StartedAt,
            FinishedAt = progress.FinishedAt
        };
    }

    public async Task<List<ReadingProgressDto>> GetCurrentlyReadingAsync(
        int userId, CancellationToken cancellationToken = default)
    {
        var progressList = await _unitOfWork.ReadingProgresses.FindAsync(
            rp => rp.UserId == userId && rp.FinishedAt == null, cancellationToken);

        return progressList.Select(p => new ReadingProgressDto
        {
            Id = p.Id,
            UserId = p.UserId,
            IssueId = p.IssueId,
            IssueTitle = p.Issue?.Title ?? "",
            SeriesName = p.Issue?.Series?.Name ?? "",
            IssueNumber = p.Issue?.IssueNumber ?? "",
            CoverUrl = p.Issue?.CoverUrl,
            CurrentPage = p.CurrentPage,
            ProgressPercent = p.ProgressPercent,
            StartedAt = p.StartedAt,
            FinishedAt = p.FinishedAt
        }).ToList();
    }
}