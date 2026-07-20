using Rest.Application.Dtos.DashboardDtos;

namespace Rest.Application.Interfaces.IServices
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(int trendDays = 7);
    }
}
