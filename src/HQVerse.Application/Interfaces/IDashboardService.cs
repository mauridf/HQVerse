using HQVerse.Application.DTOs.Dashboard;

namespace HQVerse.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default);
}