using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Publishers;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Exceptions;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PublisherService> _logger;

    public PublisherService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PublisherService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<PublisherDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var publishers = await _unitOfWork.Publishers.GetAllAsync(cancellationToken);
        var totalCount = await _unitOfWork.Publishers.CountAsync(p => true, cancellationToken);

        var paginatedItems = publishers
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<PublisherDto>
        {
            Items = _mapper.Map<IEnumerable<PublisherDto>>(paginatedItems),
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<PublisherDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var publisher = await _unitOfWork.Publishers.GetByIdAsync(id, cancellationToken);
        return publisher is null ? null : _mapper.Map<PublisherDto>(publisher);
    }

    public async Task<PublisherDto> CreateAsync(CreatePublisherDto dto, CancellationToken cancellationToken = default)
    {
        var publisher = _mapper.Map<Publisher>(dto);
        publisher.CreatedAt = DateTime.UtcNow;
        publisher.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Publishers.AddAsync(publisher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Publisher created: {PublisherName} (ID: {PublisherId})", publisher.Name, publisher.Id);
        return _mapper.Map<PublisherDto>(publisher);
    }

    public async Task<PublisherDto?> UpdateAsync(int id, UpdatePublisherDto dto, CancellationToken cancellationToken = default)
    {
        var publisher = await _unitOfWork.Publishers.GetByIdAsync(id, cancellationToken);
        if (publisher is null) return null;

        _mapper.Map(dto, publisher);
        publisher.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Publishers.UpdateAsync(publisher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PublisherDto>(publisher);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var publisher = await _unitOfWork.Publishers.GetByIdAsync(id, cancellationToken);
        if (publisher is null) return false;

        await _unitOfWork.Publishers.DeleteAsync(publisher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}