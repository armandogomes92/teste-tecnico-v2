using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Thunders.TechTest.ApiService.Services;

namespace Thunders.TechTest.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // --- Endpoints to Trigger Report Generation ---

        [HttpPost("generate/hourly-city-revenue")]
        public async Task<IActionResult> GenerateHourlyCityRevenueReport([FromQuery,Required] DateOnly forDate)
        {
            await _reportService.GenerateHourlyCityRevenueReportAsync(forDate);
            return Accepted($"Report generation for hourly city revenue on {forDate} has been queued.");
        }

        [HttpPost("generate/top-earning-toll-plazas")]
        public async Task<IActionResult> GenerateTopEarningTollPlazasReport([FromQuery,Required] int year, [FromQuery,Required] int month, [FromQuery,Required,Range(1, 100)] int topN)
        {
            await _reportService.GenerateTopEarningTollPlazasReportAsync(year, month, topN);
            return Accepted($"Report generation for top {topN} earning toll plazas for {year}-{month} has been queued.");
        }

        [HttpPost("generate/vehicle-count-by-toll-plaza")]
        public async Task<IActionResult> GenerateVehicleCountByTollPlazaReport([FromQuery,Required,StringLength(100)] string tollPlazaName, [FromQuery,Required] DateOnly forDate)
        {
            await _reportService.GenerateVehicleCountByTollPlazaReportAsync(tollPlazaName, forDate);
            return Accepted($"Report generation for vehicle count at {tollPlazaName} on {forDate} has been queued.");
        }

        // --- Endpoints to Retrieve Report Data ---

        [HttpGet("hourly-city-revenue")]
        public async Task<IActionResult> GetHourlyCityRevenueReport([FromQuery,Required] DateOnly forDate, [FromQuery,Required,StringLength(100)] string city, [FromQuery,Required,StringLength(2)] string state)
        {
            var reportData = await _reportService.GetHourlyCityRevenueReportAsync(forDate, city, state);
            if (reportData == null || !reportData.Any())
            {
                return NotFound();
            }
            return Ok(reportData);
        }

        [HttpGet("top-earning-toll-plazas")]
        public async Task<IActionResult> GetTopEarningTollPlazasReport([FromQuery,Required] int year, [FromQuery,Required] int month, [FromQuery,Required,Range(1,100)] int count = 10) // Default to top 10 if not specified
        {
            var reportData = await _reportService.GetTopEarningTollPlazasReportAsync(year, month, count);
            if (reportData == null || !reportData.Any())
            {
                return NotFound();
            }
            return Ok(reportData);
        }

        [HttpGet("vehicle-count-by-toll-plaza")]
        public async Task<IActionResult> GetVehicleCountByTollPlazaReport([FromQuery,Required,StringLength(100)] string tollPlazaName, [FromQuery,Required] DateOnly forDate)
        {
            var reportData = await _reportService.GetVehicleCountByTollPlazaReportAsync(tollPlazaName, forDate);
            if (reportData == null || !reportData.Any())
            {
                return NotFound();
            }
            return Ok(reportData);
        }
    }
}