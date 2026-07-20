using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Interfaces.IServices;
using Swashbuckle.AspNetCore.Annotations;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Retrieves aggregated KPI cards, status breakdown, revenue trend,
        /// and top-selling dishes for the Admin dashboard.
        /// </summary>
        /// <param name="trendDays">Number of days to include in the revenue trend (default 7)</param>
        [HttpGet("stats")]
        [SwaggerOperation(Summary = "Get dashboard stats", Description = "Admin only.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dashboard stats retrieved successfully")]
        public async Task<IActionResult> GetStats([FromQuery] int trendDays = 7)
        {
            var stats = await _dashboardService.GetDashboardStatsAsync(trendDays);
            return SuccessResponse(stats, "Dashboard stats retrieved successfully");
        }
    }
}
