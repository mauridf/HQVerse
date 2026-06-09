using HQVerse.Application.DTOs.Search;

namespace HQVerse.Application.Interfaces;

public interface ISearchService
{
    Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default);
}