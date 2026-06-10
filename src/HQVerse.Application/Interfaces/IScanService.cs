using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Scans;

namespace HQVerse.Application.Interfaces;

public interface IScanService
{
    // ScanGroups
    Task<List<ScanGroupDto>> GetAllScanGroupsAsync(CancellationToken cancellationToken = default);
    Task<ScanGroupDto?> GetScanGroupByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ScanGroupDto> CreateScanGroupAsync(CreateScanGroupDto dto, CancellationToken cancellationToken = default);

    // Scans
    Task<PaginatedResult<ScanDto>> GetScansByIssueIdAsync(int issueId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<ScanDetailDto?> GetScanByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ScanDto> CreateScanAsync(int uploaderUserId, CreateScanDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteScanAsync(int scanId, int userId, CancellationToken cancellationToken = default);

    // Busca
    Task<PaginatedResult<ScanDto>> SearchScansAsync(string query, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<PaginatedResult<ScanDto>> GetLatestScansAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
}