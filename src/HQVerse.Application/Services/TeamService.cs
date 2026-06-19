using AutoMapper;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Teams;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HQVerse.Application.Services;

public class TeamService : ITeamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TeamService> _logger;

    public TeamService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TeamService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<TeamDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var teams = await _unitOfWork.Teams.GetAllAsync(cancellationToken);
        var totalCount = await _unitOfWork.Teams.CountAsync(t => true, cancellationToken);

        var paginatedItems = teams
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToList();

        return new PaginatedResult<TeamDto>
        {
            Items = _mapper.Map<IEnumerable<TeamDto>>(paginatedItems),
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<TeamDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(id, cancellationToken);
        return team is null ? null : _mapper.Map<TeamDto>(team);
    }

    public async Task<IEnumerable<TeamDto>> SearchByNameAsync(string query, CancellationToken cancellationToken = default)
    {
        var teams = await _unitOfWork.Teams.FindAsync(t => t.Name.Contains(query), cancellationToken);
        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto dto, CancellationToken cancellationToken = default)
    {
        var team = _mapper.Map<Team>(dto);
        await _unitOfWork.Teams.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Team created: {TeamName} (ID: {TeamId})", team.Name, team.Id);
        return _mapper.Map<TeamDto>(team);
    }

    public async Task<TeamDto?> UpdateAsync(int id, UpdateTeamDto dto, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(id, cancellationToken);
        if (team is null) return null;

        _mapper.Map(dto, team);
        await _unitOfWork.Teams.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TeamDto>(team);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(id, cancellationToken);
        if (team is null) return false;

        await _unitOfWork.Teams.DeleteAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
