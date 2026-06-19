using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Universes;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class UniverseService : IUniverseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UniverseService> _logger;

    public UniverseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UniverseService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<UniverseDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var universes = await _unitOfWork.Universes.GetAllAsync(cancellationToken);
        var totalCount = await _unitOfWork.Universes.CountAsync(u => true, cancellationToken);

        var paginatedItems = universes
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<UniverseDto>
        {
            Items = _mapper.Map<IEnumerable<UniverseDto>>(paginatedItems),
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<UniverseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var universe = await _unitOfWork.Universes.GetByIdAsync(id, cancellationToken);
        return universe is null ? null : _mapper.Map<UniverseDto>(universe);
    }

    public async Task<UniverseDto> CreateAsync(CreateUniverseDto dto, CancellationToken cancellationToken = default)
    {
        var universe = _mapper.Map<Universe>(dto);
        await _unitOfWork.Universes.AddAsync(universe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Universe created: {UniverseName} (ID: {UniverseId})", universe.Name, universe.Id);
        return _mapper.Map<UniverseDto>(universe);
    }

    public async Task<UniverseDto?> UpdateAsync(int id, UpdateUniverseDto dto, CancellationToken cancellationToken = default)
    {
        var universe = await _unitOfWork.Universes.GetByIdAsync(id, cancellationToken);
        if (universe is null) return null;

        _mapper.Map(dto, universe);
        await _unitOfWork.Universes.UpdateAsync(universe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UniverseDto>(universe);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var universe = await _unitOfWork.Universes.GetByIdAsync(id, cancellationToken);
        if (universe is null) return false;

        await _unitOfWork.Universes.DeleteAsync(universe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
