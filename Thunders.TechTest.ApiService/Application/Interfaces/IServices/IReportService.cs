using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.ApiService.Domain.Enums;
using Thunders.TechTest.ApiService.Domain.Models.Reports;

namespace Thunders.TechTest.ApiService.Application.Interfaces.IServices;
public interface IReportService
{
    Task<Guid> GenerateHourlyCityRevenueReportAsync(DateOnly forDate);
    Task<Guid> GenerateTopEarningTollPlazasReportAsync(int year, int month, int quantityOfPlazas);
    Task<Guid> GenerateVehicleCountByTollPlazaReportAsync(int tollPlazaId, DateOnly reportDate);

    Task<ReportStatus> GetHourlyCityRevenueReportStatusAsync(Guid reportId);
    Task<IEnumerable<HourlyCityRevenue>> GetHourlyCityRevenueReportResultAsync(Guid reportId);
    Task<ReportStatus> GetTopEarningTollPlazasReportStatusAsync(Guid reportId);
    Task<IEnumerable<TopEarningTollPlaza>> GetTopEarningTollPlazasReportResultAsync(Guid reportId);
    Task<ReportStatus> GetVehicleCountByTollPlazaReportStatusAsync(Guid reportId);
    Task<IEnumerable<VehicleCountByTollPlaza>> GetVehicleCountByTollPlazaReportResultAsync(Guid reportId);
}