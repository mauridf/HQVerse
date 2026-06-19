using HQVerse.Application.DTOs.ComicVine;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class ComicVineSyncService : IComicVineService
{
    private readonly IComicVineClient _client;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ComicVineSyncService> _logger;

    public ComicVineSyncService(IComicVineClient client, IUnitOfWork unitOfWork, ILogger<ComicVineSyncService> logger)
    {
        _client = client;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<ComicVineSearchResult>> SearchAsync(
        string query, string resourceType, CancellationToken cancellationToken = default)
    {
        var response = await _client.SearchAsync(resourceType, query, 20, cancellationToken);
        return response.Results;
    }

    public async Task<ComicVineSearchResult?> GetByIdAsync(
        string resourceType, int comicVineId, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetByIdAsync(resourceType, comicVineId, cancellationToken);
        if (response.Results.ValueKind == System.Text.Json.JsonValueKind.Object)
            return System.Text.Json.JsonSerializer.Deserialize<ComicVineSearchResult>(response.Results.GetRawText());
        return null;
    }

    // ==================== PUBLISHERS ====================
    public async Task SyncPublisherAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Publishers.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null) return;

        var detail = await _client.GetDetailAsync<ComicVinePublisherDetail>("publisher", comicVineId, cancellationToken);
        if (detail is null || detail.Id == 0)
            throw new InvalidOperationException($"Publisher not found on Comic Vine: {comicVineId}");

        var publisher = new Publisher
        {
            ComicVineId = detail.Id,
            Name = detail.Name ?? "Unknown",
            Description = detail.Description ?? detail.Deck,
            Website = detail.SiteDetailUrl,
            LogoUrl = detail.Image?.MediumUrl,
            BannerUrl = detail.Image?.SuperUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Publishers.AddAsync(publisher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Publisher synced: {Name} (CV ID: {ComicVineId})", publisher.Name, comicVineId);
    }

    // ==================== CHARACTERS ====================
    public async Task SyncCharacterAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Characters.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null) return;

        var detail = await _client.GetDetailAsync<ComicVineCharacterDetail>("character", comicVineId, cancellationToken);
        if (detail is null || detail.Id == 0)
            throw new InvalidOperationException($"Character not found on Comic Vine: {comicVineId}");

        int? publisherId = null;
        if (detail.Publisher is not null)
        {
            var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(detail.Publisher.Id, cancellationToken);
            publisherId = pub?.Id;
        }

        var character = new Character
        {
            ComicVineId = detail.Id,
            Name = detail.Name ?? "Unknown",
            RealName = detail.RealName,
            Description = detail.Description ?? detail.Deck,
            Gender = detail.Gender?.ValueKind == System.Text.Json.JsonValueKind.Number
                        ? (detail.Gender.Value.GetInt32() switch { 1 => "Male", 2 => "Female", _ => null })
                        : null,
            ImageUrl = detail.Image?.MediumUrl,
            ThumbnailUrl = detail.Image?.ThumbUrl,
            PublisherId = publisherId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Characters.AddAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Character synced: {Name} (CV ID: {ComicVineId})", character.Name, comicVineId);
    }

    // ==================== TEAMS ====================
    public async Task SyncTeamAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Teams.FindAsync(t => t.ComicVineId == comicVineId, cancellationToken);
        if (existing.Any()) return;

        var detail = await _client.GetDetailAsync<ComicVineTeamDetail>("team", comicVineId, cancellationToken);
        if (detail is null || detail.Id == 0)
            throw new InvalidOperationException($"Team not found on Comic Vine: {comicVineId}");

        int? publisherId = null;
        if (detail.Publisher is not null)
        {
            var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(detail.Publisher.Id, cancellationToken);
            publisherId = pub?.Id;
        }

        var team = new Team
        {
            ComicVineId = detail.Id,
            Name = detail.Name ?? "Unknown",
            Description = detail.Description ?? detail.Deck,
            ImageUrl = detail.Image?.MediumUrl,
            PublisherId = publisherId
        };

        await _unitOfWork.Teams.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Team synced: {Name} (CV ID: {ComicVineId})", team.Name, comicVineId);
    }

    // ==================== CREATORS ====================
    public async Task SyncCreatorAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Creators.FindAsync(c => c.ComicVineId == comicVineId, cancellationToken);
        if (existing.Any()) return;

        var detail = await _client.GetDetailAsync<ComicVineCreatorDetail>("person", comicVineId, cancellationToken);
        if (detail is null || detail.Id == 0)
            throw new InvalidOperationException($"Creator not found on Comic Vine: {comicVineId}");

        var creator = new Creator
        {
            ComicVineId = detail.Id,
            Name = detail.Name ?? "Unknown",
            Description = detail.Description ?? detail.Deck,
            ImageUrl = detail.Image?.MediumUrl,
            BirthDate = DateTime.TryParse(detail.Birth, out var bd) ? bd : null
        };

        await _unitOfWork.Creators.AddAsync(creator, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Creator synced: {Name} (CV ID: {ComicVineId})", creator.Name, comicVineId);
    }

    // ==================== VOLUMES ====================
    public async Task SyncVolumeAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.ComicSeries.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null) return;

        var detail = await _client.GetDetailAsync<ComicVineVolumeDetail>("volume", comicVineId, cancellationToken);
        if (detail is null || detail.Id == 0)
            throw new InvalidOperationException($"Volume not found on Comic Vine: {comicVineId}");

        int? publisherId = null;
        if (detail.Publisher is not null)
        {
            try { await SyncPublisherAsync(detail.Publisher.Id, cancellationToken); } catch { }
            var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(detail.Publisher.Id, cancellationToken);
            publisherId = pub?.Id;
        }

        var series = new ComicSeries
        {
            ComicVineId = detail.Id,
            Name = detail.Name ?? "Unknown",
            Description = detail.Description ?? detail.Deck,
            ImageUrl = detail.Image?.MediumUrl,
            BannerUrl = detail.Image?.SuperUrl,
            PublisherId = publisherId,
            StartYear = int.TryParse(detail.StartYear, out var sy) ? sy : null,
            TotalIssues = detail.CountOfIssues
        };

        await _unitOfWork.ComicSeries.AddAsync(series, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Volume synced: {Name} (CV ID: {ComicVineId})", series.Name, comicVineId);
    }

    // ==================== ISSUES ====================
    public async Task SyncIssueAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.ComicIssues.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null) return;

        var detail = await _client.GetDetailAsync<ComicVineIssueDetail>("issue", comicVineId, cancellationToken);
        if (detail is null || detail.Id == 0)
            throw new InvalidOperationException($"Issue not found on Comic Vine: {comicVineId}");

        int? seriesId = null;
        if (detail.Volume is not null)
        {
            try { await SyncVolumeAsync(detail.Volume.Id, cancellationToken); } catch { }
            var vol = await _unitOfWork.ComicSeries.GetByComicVineIdAsync(detail.Volume.Id, cancellationToken);
            seriesId = vol?.Id;
        }

        if (seriesId is null)
            throw new InvalidOperationException($"Series not found for issue {comicVineId}");

        var issue = new ComicIssue
        {
            ComicVineId = detail.Id,
            SeriesId = seriesId.Value,
            IssueNumber = detail.IssueNumber ?? "0",
            Title = detail.Name,
            Synopsis = detail.Description,
            CoverUrl = detail.Image?.MediumUrl,
            ThumbnailUrl = detail.Image?.ThumbUrl,
            CoverDate = DateTime.TryParse(detail.CoverDate, out var cd) ? cd : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ComicIssues.AddAsync(issue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Issue synced: ID {Id} (CV ID: {ComicVineId})", issue.Id, comicVineId);
    }

    // ==================== STORY ARCS ====================
    public async Task SyncStoryArcAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.StoryArcs.FindAsync(sa => sa.ComicVineId == comicVineId, cancellationToken);
        if (existing.Any()) return;

        var detail = await _client.GetDetailAsync<ComicVineStoryArcDetail>("story_arc", comicVineId, cancellationToken);
        if (detail is null || detail.Id == 0)
            throw new InvalidOperationException($"StoryArc not found on Comic Vine: {comicVineId}");

        int? publisherId = null;
        if (detail.Publisher is not null)
        {
            var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(detail.Publisher.Id, cancellationToken);
            publisherId = pub?.Id;
        }

        var storyArc = new StoryArc
        {
            ComicVineId = detail.Id,
            Name = detail.Name ?? "Unknown",
            Description = detail.Description ?? detail.Deck,
            ImageUrl = detail.Image?.MediumUrl,
            PublisherId = publisherId
        };

        await _unitOfWork.StoryArcs.AddAsync(storyArc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("StoryArc synced: {Name} (CV ID: {ComicVineId})", storyArc.Name, comicVineId);
    }
}