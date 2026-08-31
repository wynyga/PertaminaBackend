using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Semua endpoint dashboard memerlukan login
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(summary);
        }
    }
}
