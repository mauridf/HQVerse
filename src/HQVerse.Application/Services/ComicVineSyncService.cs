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
        return response.Results;
    }

    // ==================== PUBLISHERS ====================
    public async Task SyncPublisherAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Publishers.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Publisher already synced: CV ID {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("publisher", comicVineId, cancellationToken);
        var data = response.Results;

        var publisher = new Publisher
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            Description = data.Description,
            Website = data.SiteDetailUrl,
            LogoUrl = data.Image?.MediumUrl,
            BannerUrl = data.Image?.SuperUrl,
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
        if (existing is not null)
        {
            _logger.LogInformation("Character already synced: CV ID {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("character", comicVineId, cancellationToken);
        var data = response.Results;

        // Buscar publisher se existir
        int? publisherId = null;
        if (data.Publisher is not null)
        {
            var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(data.Publisher.Id, cancellationToken);
            publisherId = pub?.Id;
        }

        var character = new Character
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            RealName = data.RealName,
            Description = data.Description,
            Gender = data.Gender switch { 1 => "Male", 2 => "Female", _ => null },
            ImageUrl = data.Image?.MediumUrl,
            ThumbnailUrl = data.Image?.ThumbUrl,
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
        var existing = await _unitOfWork.Teams.FindAsync(
            t => t.ComicVineId == comicVineId, cancellationToken);

        if (existing.Any())
        {
            _logger.LogInformation("Team already synced: CV ID {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("team", comicVineId, cancellationToken);
        var data = response.Results;

        int? publisherId = null;
        if (data.Publisher is not null)
        {
            var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(data.Publisher.Id, cancellationToken);
            publisherId = pub?.Id;
        }

        var team = new Team
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            Description = data.Description,
            ImageUrl = data.Image?.MediumUrl,
            PublisherId = publisherId
        };

        await _unitOfWork.Teams.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Team synced: {Name} (CV ID: {ComicVineId})", team.Name, comicVineId);
    }

    // ==================== CREATORS (PEOPLE) ====================
    public async Task SyncCreatorAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Creators.FindAsync(
            c => c.ComicVineId == comicVineId, cancellationToken);

        if (existing.Any())
        {
            _logger.LogInformation("Creator already synced: CV ID {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("person", comicVineId, cancellationToken);
        var data = response.Results;

        var creator = new Creator
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            Description = data.Description,
            ImageUrl = data.Image?.MediumUrl,
            BirthDate = DateTime.TryParse(data.Birth, out var bd) ? bd : null
        };

        await _unitOfWork.Creators.AddAsync(creator, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Creator synced: {Name} (CV ID: {ComicVineId})", creator.Name, comicVineId);
    }

    // ==================== VOLUMES (COMIC SERIES) ====================
    public async Task SyncVolumeAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.ComicSeries.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Volume already synced: CV ID {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("volume", comicVineId, cancellationToken);
        var data = response.Results;

        // Sincronizar publisher primeiro se existir
        int? publisherId = null;
        if (data.Publisher is not null)
        {
            try
            {
                await SyncPublisherAsync(data.Publisher.Id, cancellationToken);
                var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(data.Publisher.Id, cancellationToken);
                publisherId = pub?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync publisher {PublisherId} for volume", data.Publisher.Id);
            }
        }

        var series = new ComicSeries
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            Description = data.Description,
            ImageUrl = data.Image?.MediumUrl,
            BannerUrl = data.Image?.SuperUrl,
            PublisherId = publisherId,
            StartYear = int.TryParse(data.StartYear, out var sy) ? sy : null,
            TotalIssues = data.CountOfIssues
        };

        await _unitOfWork.ComicSeries.AddAsync(series, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volume synced: {Name} (CV ID: {ComicVineId})", series.Name, comicVineId);
    }

    // ==================== ISSUES ====================
    public async Task SyncIssueAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.ComicIssues.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Issue already synced: CV ID {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("issue", comicVineId, cancellationToken);
        var data = response.Results;

        // Sincronizar volume primeiro
        int? seriesId = null;
        if (data.Volume is not null)
        {
            try
            {
                await SyncVolumeAsync(data.Volume.Id, cancellationToken);
                var vol = await _unitOfWork.ComicSeries.GetByComicVineIdAsync(data.Volume.Id, cancellationToken);
                seriesId = vol?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync volume {VolumeId} for issue", data.Volume.Id);
            }
        }

        if (seriesId is null)
            throw new InvalidOperationException($"Series not found for issue {comicVineId}");

        var issue = new ComicIssue
        {
            ComicVineId = data.Id,
            SeriesId = seriesId.Value,
            IssueNumber = data.IssueNumber ?? "0",
            Title = data.Name,
            Synopsis = data.Description,
            CoverUrl = data.Image?.MediumUrl,
            ThumbnailUrl = data.Image?.ThumbUrl,
            CoverDate = DateTime.TryParse(data.CoverDate, out var cd) ? cd : null,
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
        var existing = await _unitOfWork.StoryArcs.FindAsync(
            sa => sa.ComicVineId == comicVineId, cancellationToken);

        if (existing.Any())
        {
            _logger.LogInformation("StoryArc already synced: CV ID {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("story_arc", comicVineId, cancellationToken);
        var data = response.Results;

        int? publisherId = null;
        if (data.Publisher is not null)
        {
            var pub = await _unitOfWork.Publishers.GetByComicVineIdAsync(data.Publisher.Id, cancellationToken);
            publisherId = pub?.Id;
        }

        var storyArc = new StoryArc
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            Description = data.Description,
            ImageUrl = data.Image?.MediumUrl,
            PublisherId = publisherId
        };

        await _unitOfWork.StoryArcs.AddAsync(storyArc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("StoryArc synced: {Name} (CV ID: {ComicVineId})", storyArc.Name, comicVineId);
    }
}