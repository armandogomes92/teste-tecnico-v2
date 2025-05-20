using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Thunders.TechTest.ApiService.Application.Interfaces.IServices;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
namespace Thunders.TechTest.ApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(IReportService reportService) : ControllerBase
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    // --- Endpoints to Trigger Report Generation ---

    [HttpPost("generate/hourly-city-revenue")]
    public async Task<IActionResult> GenerateHourlyCityRevenueReport([FromQuery] DateOnly forDate)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var reportId = await reportService.GenerateHourlyCityRevenueReportAsync(forDate).WaitAsync(DefaultTimeout, cts.Token);
            return Accepted(new { ReportId = reportId });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para geração do relatório.");
        }
    }

    [HttpPost("generate/top-earning-toll-plazas")]
    public async Task<IActionResult> GenerateTopEarningTollPlazasReport([FromQuery,Required] int year, [FromQuery,Required] int month, [FromQuery,Required,Range(1, 100)] int quantityOfPlazas)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var reportId = await reportService.GenerateTopEarningTollPlazasReportAsync(year, month, quantityOfPlazas).WaitAsync(DefaultTimeout, cts.Token);
            return Accepted(new { ReportId = reportId });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para geração do relatório.");
        }
    }

    [HttpPost("generate/vehicle-count-by-toll-plaza")]
    public async Task<IActionResult> GenerateVehicleCountByTollPlazaReport([FromQuery,Required] int tollPlazaId, [FromQuery,Required] DateOnly reportDate)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var reportId = await reportService.GenerateVehicleCountByTollPlazaReportAsync(tollPlazaId, reportDate).WaitAsync(DefaultTimeout, cts.Token);
            return Accepted(new { ReportId = reportId });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para geração do relatório.");
        }
    }

     // --- Endpoints to Report Status --

    [HttpGet("status/hourly-city-revenue/{reportId}")]
    public async Task<IActionResult> GetHourlyCityRevenueReportStatus([FromRoute] Guid reportId)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var status = await reportService.GetHourlyCityRevenueReportStatusAsync(reportId).WaitAsync(DefaultTimeout, cts.Token);
            return Ok(new { ReportId = reportId, Status = status.ToString() });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para consulta de status do relatório.");
        }
    }

    [HttpGet("status/top-earning-toll-plazas/{reportId}")]
    public async Task<IActionResult> GetTopEarningTollPlazasReportStatus([FromRoute] Guid reportId)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var status = await reportService.GetTopEarningTollPlazasReportStatusAsync(reportId).WaitAsync(DefaultTimeout, cts.Token);
            return Ok(new { ReportId = reportId, Status = status.ToString() });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para consulta de status do relatório.");
        }
    }

    [HttpGet("status/vehicle-count-by-toll-plaza/{reportId}")]
    public async Task<IActionResult> GetVehicleCountByTollPlazaReportStatus([FromRoute] Guid reportId)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var status = await reportService.GetVehicleCountByTollPlazaReportStatusAsync(reportId).WaitAsync(DefaultTimeout, cts.Token);
            return Ok(new { ReportId = reportId, Status = status.ToString() });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para consulta de status do relatório.");
        }
    }

    // --- Endpoints to Report Result --

    [HttpGet("result/hourly-city-revenue/{reportId}")]
    public async Task<IActionResult> GetHourlyCityRevenueReportResult([FromRoute] Guid reportId)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var result = await reportService.GetHourlyCityRevenueReportResultAsync(reportId).WaitAsync(DefaultTimeout, cts.Token);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para consulta do resultado do relatório.");
        }
    }
    
    [HttpGet("result/top-earning-toll-plazas/{reportId}")]
    public async Task<IActionResult> GetTopEarningTollPlazasReportResult([FromRoute] Guid reportId)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var result = await reportService.GetTopEarningTollPlazasReportResultAsync(reportId).WaitAsync(DefaultTimeout, cts.Token);
            if (result == null || !result.Any())
                return NotFound();
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para consulta do resultado do relatório.");
        }
    }

    [HttpGet("result/vehicle-count-by-toll-plaza/{reportId}")]
    public async Task<IActionResult> GetVehicleCountByTollPlazaReportResult([FromRoute] Guid reportId)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        try
        {
            var result = await reportService.GetVehicleCountByTollPlazaReportResultAsync(reportId).WaitAsync(DefaultTimeout, cts.Token);
            if (result == null || !result.Any())
                return NotFound();
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, "Tempo limite excedido para consulta do resultado do relatório.");
        }
    }
}