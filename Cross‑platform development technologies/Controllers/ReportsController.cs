using Cross_platform_development_technologies.Contracts;
using Cross_platform_development_technologies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cross_platform_development_technologies.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("gauges")]
        public async Task<ActionResult<List<GaugeSummaryResponse>>>
            GetGaugeSummaries()
        {
            var summaries = await _reportService.GetGaugeSummariesAsync();

            return Ok(summaries);
        }
    }
}