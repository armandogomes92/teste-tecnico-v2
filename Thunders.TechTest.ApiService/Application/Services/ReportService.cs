using Thunders.TechTest.ApiService.Application.Interfaces.IServices;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.ApiService.Domain.Models.Reports;
using Thunders.TechTest.ApiService.Infrastructure.Messages;
using Thunders.TechTest.OutOfBox.Queues;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Application.Services;

public class ReportService : IReportService
{
    private readonly IMessageSender _messageSender;
    private readonly ILogger<ReportService> _logger;
    private readonly IReportCacheRepository _cacheRepository;

    public ReportService(ILogger<ReportService> logger, IMessageSender messageSender, IReportCacheRepository cacheRepository)
    {
        _logger = logger;
        _messageSender = messageSender;
        _cacheRepository = cacheRepository;
    }

    public async Task<Guid> GenerateHourlyCityRevenueReportAsync(DateOnly forDate)
    {
        using var activity = Telemetry.Source.StartActivity("GenerateHourlyCityRevenueReport");
        activity?.SetTag("report.forDate", forDate.ToString());
        var reportId = Guid.NewGuid();
        await _messageSender.SendLocal(new HourlyCityRevenueReportMessage { ForDate = forDate, ReportId = reportId });
        activity?.SetTag("report.id", reportId);
        return reportId;
    }

    public async Task<Guid> GenerateTopEarningTollPlazasReportAsync(int year, int month, int quantityOfPlazas)
    {
        var reportId = Guid.NewGuid();
        await _messageSender.SendLocal(new TopEarningTollPlazasReportMessage { Year = year, Month = month, quantityOfPlazas = quantityOfPlazas, ReportId = reportId });
        return reportId;
    }

    public async Task<Guid> GenerateVehicleCountByTollPlazaReportAsync(int tollPlazaId, DateOnly reportDate)
    {
        var reportId = Guid.NewGuid();
        await _messageSender.SendLocal(new VehicleCountByTollPlazaReportMessage { TollPlazaId = tollPlazaId, ReportDate = reportDate, ReportId = reportId });
        return reportId;
    }

    public async Task<ReportStatus> GetHourlyCityRevenueReportStatusAsync(Guid reportId)
    {
        var status = await _cacheRepository.GetAsync<ReportStatus>($"status:{reportId}");
        return status;
    }

    public async Task<IEnumerable<HourlyCityRevenue>> GetHourlyCityRevenueReportResultAsync(Guid reportId)
    {
        var result = await _cacheRepository.GetAsync<List<HourlyCityRevenue>>(reportId.ToString());
        return result ?? new List<HourlyCityRevenue>();
    }

    public async Task<ReportStatus> GetTopEarningTollPlazasReportStatusAsync(Guid reportId)
    {
        var status = await _cacheRepository.GetAsync<ReportStatus>($"status:{reportId}");
        return status;
    }

    public async Task<IEnumerable<TopEarningTollPlaza>> GetTopEarningTollPlazasReportResultAsync(Guid reportId)
    {
        var result = await _cacheRepository.GetAsync<List<TopEarningTollPlaza>>(reportId.ToString());
        return result ?? new List<TopEarningTollPlaza>();
    }

    public async Task<ReportStatus> GetVehicleCountByTollPlazaReportStatusAsync(Guid reportId)
    {
        var status = await _cacheRepository.GetAsync<ReportStatus>($"status:{reportId}");
        return status;
    }

    public async Task<IEnumerable<VehicleCountByTollPlaza>> GetVehicleCountByTollPlazaReportResultAsync(Guid reportId)
    {
        var result = await _cacheRepository.GetAsync<List<VehicleCountByTollPlaza>>(reportId.ToString());
        return result ?? new List<VehicleCountByTollPlaza>();
    }
}