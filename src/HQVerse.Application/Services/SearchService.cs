using AutoMapper;
using HQVerse.Application.DTOs.Search;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SearchService> _logger;

    public SearchService(IUnitOfWork unitOfWork, ILogger<SearchService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new SearchResultDto();

        _logger.LogInformation("Searching for: {Query}", query);

        var characters = await _unitOfWork.Characters.SearchByNameAsync(query, cancellationToken);
        var comicSeries = await _unitOfWork.ComicSeries.SearchByNameAsync(query, cancellationToken);
        var comicIssues = await _unitOfWork.ComicIssues.SearchAsync(query, cancellationToken);
        var publishers = await _unitOfWork.Publishers.SearchByNameAsync(query, cancellationToken);

        return new SearchResultDto
        {
            Characters = characters.Select(c => new SearchItemDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = "Character",
                ImageUrl = c.ThumbnailUrl,
                Subtitle = c.Publisher?.Name
            }).Take(10).ToList(),

            ComicSeries = comicSeries.Select(cs => new SearchItemDto
            {
                Id = cs.Id,
                Name = cs.Name,
                Type = "ComicSeries",
                ImageUrl = cs.ImageUrl,
                Subtitle = cs.StartYear?.ToString()
            }).Take(10).ToList(),

            ComicIssues = comicIssues.Select(ci => new SearchItemDto
            {
                Id = ci.Id,
                Name = $"{ci.Series.Name} #{ci.IssueNumber}",
                Type = "ComicIssue",
                ImageUrl = ci.ThumbnailUrl,
                Subtitle = ci.Title
            }).Take(10).ToList(),

            Publishers = publishers.Select(p => new SearchItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = "Publisher",
                ImageUrl = p.LogoUrl,
                Subtitle = p.Country
            }).Take(10).ToList()
        };
    }
}