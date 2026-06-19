using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.ComicSeries;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class ComicSeriesService : IComicSeriesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ComicSeriesService> _logger;

    public ComicSeriesService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ComicSeriesService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<ComicSeriesDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var series = await _unitOfWork.ComicSeries.GetAllAsync(cancellationToken);
        var totalCount = await _unitOfWork.ComicSeries.CountAsync(s => true, cancellationToken);

        var paginatedItems = series
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<ComicSeriesDto>
        {
            Items = _mapper.Map<IEnumerable<ComicSeriesDto>>(paginatedItems),
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<ComicSeriesDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var series = await _unitOfWork.ComicSeries.GetWithIssuesAsync(id, cancellationToken);
        return series is null ? null : _mapper.Map<ComicSeriesDto>(series);
    }

    public async Task<IEnumerable<ComicSeriesDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default)
    {
        var series = await _unitOfWork.ComicSeries.SearchByNameAsync(query, cancellationToken);
        return _mapper.Map<IEnumerable<ComicSeriesDto>>(series);
    }

    public async Task<ComicSeriesDto> CreateAsync(CreateComicSeriesDto dto, CancellationToken cancellationToken = default)
    {
        var series = _mapper.Map<ComicSeries>(dto);
        await _unitOfWork.ComicSeries.AddAsync(series, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ComicSeries created: {SeriesName} (ID: {SeriesId})", series.Name, series.Id);
        return _mapper.Map<ComicSeriesDto>(series);
    }

    public async Task<ComicSeriesDto?> UpdateAsync(int id, CreateComicSeriesDto dto, CancellationToken cancellationToken = default)
    {
        var series = await _unitOfWork.ComicSeries.GetByIdAsync(id, cancellationToken);
        if (series is null) return null;

        _mapper.Map(dto, series);
        await _unitOfWork.ComicSeries.UpdateAsync(series, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ComicSeriesDto>(series);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var series = await _unitOfWork.ComicSeries.GetByIdAsync(id, cancellationToken);
        if (series is null) return false;

        await _unitOfWork.ComicSeries.DeleteAsync(series, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
