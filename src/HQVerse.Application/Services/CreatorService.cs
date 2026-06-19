using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Creators;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class CreatorService : ICreatorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreatorService> _logger;

    public CreatorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreatorService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<CreatorDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var creators = await _unitOfWork.Creators.GetAllAsync(cancellationToken);
        var totalCount = await _unitOfWork.Creators.CountAsync(c => true, cancellationToken);

        var paginatedItems = creators
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<CreatorDto>
        {
            Items = _mapper.Map<IEnumerable<CreatorDto>>(paginatedItems),
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<CreatorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var creator = await _unitOfWork.Creators.GetByIdAsync(id, cancellationToken);
        return creator is null ? null : _mapper.Map<CreatorDto>(creator);
    }

    public async Task<IEnumerable<CreatorDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default)
    {
        var creators = await _unitOfWork.Creators.FindAsync(c => c.Name.Contains(query), cancellationToken);
        return _mapper.Map<IEnumerable<CreatorDto>>(creators);
    }

    public async Task<CreatorDto> CreateAsync(CreateCreatorDto dto, CancellationToken cancellationToken = default)
    {
        var creator = _mapper.Map<Creator>(dto);
        await _unitOfWork.Creators.AddAsync(creator, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Creator created: {CreatorName} (ID: {CreatorId})", creator.Name, creator.Id);
        return _mapper.Map<CreatorDto>(creator);
    }

    public async Task<CreatorDto?> UpdateAsync(int id, UpdateCreatorDto dto, CancellationToken cancellationToken = default)
    {
        var creator = await _unitOfWork.Creators.GetByIdAsync(id, cancellationToken);
        if (creator is null) return null;

        _mapper.Map(dto, creator);
        await _unitOfWork.Creators.UpdateAsync(creator, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CreatorDto>(creator);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var creator = await _unitOfWork.Creators.GetByIdAsync(id, cancellationToken);
        if (creator is null) return false;

        await _unitOfWork.Creators.DeleteAsync(creator, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

}
