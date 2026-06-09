using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.ExternalServices.ComicVine;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class ComicVineSyncService : IComicVineService
{
    private readonly ComicVineClient _client;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ComicVineSyncService> _logger;

    public ComicVineSyncService(ComicVineClient client, IUnitOfWork unitOfWork, ILogger<ComicVineSyncService> logger)
    {
        _client = client;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<Application.DTOs.ComicVine.ComicVineSearchResult>> SearchAsync(
        string query, string resourceType, CancellationToken cancellationToken = default)
    {
        var response = await _client.SearchAsync(resourceType, query, 20, cancellationToken);
        return response.Results;
    }

    public async Task<Application.DTOs.ComicVine.ComicVineSearchResult?> GetByIdAsync(
        string resourceType, int comicVineId, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetByIdAsync(resourceType, comicVineId, cancellationToken);
        return response.Results;
    }

    public async Task SyncPublisherAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Publishers.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Publisher already synced: {ComicVineId}", comicVineId);
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Publishers.AddAsync(publisher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Publisher synced: {Name} (CV ID: {ComicVineId})", publisher.Name, comicVineId);
    }

    public async Task SyncCharacterAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Characters.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Character already synced: {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("character", comicVineId, cancellationToken);
        var data = response.Results;

        var character = new Character
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            RealName = data.RealName,
            Description = data.Description,
            ImageUrl = data.Image?.MediumUrl,
            ThumbnailUrl = data.Image?.ThumbUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Characters.AddAsync(character, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Character synced: {Name} (CV ID: {ComicVineId})", character.Name, comicVineId);
    }

    public async Task SyncVolumeAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.ComicSeries.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Volume already synced: {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("volume", comicVineId, cancellationToken);
        var data = response.Results;

        // Sincronizar publisher primeiro se existir
        if (data.Publisher is not null)
        {
            try
            {
                await SyncPublisherAsync(data.Publisher.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync publisher {PublisherId} for volume", data.Publisher.Id);
            }
        }

        // Buscar o publisher local
        var publisher = data.Publisher is not null
            ? await _unitOfWork.Publishers.GetByComicVineIdAsync(data.Publisher.Id, cancellationToken)
            : null;

        var series = new ComicSeries
        {
            ComicVineId = data.Id,
            Name = data.Name ?? "Unknown",
            Description = data.Description,
            ImageUrl = data.Image?.MediumUrl,
            PublisherId = publisher?.Id,
            StartYear = int.TryParse(data.StartYear, out var sy) ? sy : null,
            TotalIssues = data.CountOfIssues
        };

        await _unitOfWork.ComicSeries.AddAsync(series, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volume synced: {Name} (CV ID: {ComicVineId})", series.Name, comicVineId);
    }

    public async Task SyncIssueAsync(int comicVineId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.ComicIssues.GetByComicVineIdAsync(comicVineId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Issue already synced: {ComicVineId}", comicVineId);
            return;
        }

        var response = await _client.GetByIdAsync("issue", comicVineId, cancellationToken);
        var data = response.Results;

        // Sincronizar volume primeiro
        if (data.Volume is not null)
        {
            try
            {
                await SyncVolumeAsync(data.Volume.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync volume {VolumeId} for issue", data.Volume.Id);
            }
        }

        // Buscar a série local
        var series = data.Volume is not null
            ? await _unitOfWork.ComicSeries.GetByComicVineIdAsync(data.Volume.Id, cancellationToken)
            : null;

        if (series is null)
            throw new InvalidOperationException($"Series not found for issue {comicVineId}");

        var issue = new ComicIssue
        {
            ComicVineId = data.Id,
            SeriesId = series.Id,
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

        _logger.LogInformation("Issue synced: {Series}#{Number} (CV ID: {ComicVineId})", series.Name, issue.IssueNumber, comicVineId);
    }
}