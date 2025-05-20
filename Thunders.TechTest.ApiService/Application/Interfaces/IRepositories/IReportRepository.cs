using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thunders.TechTest.ApiService.Domain.Enums;
using Thunders.TechTest.ApiService.Domain.Models.Reports;

namespace Thunders.TechTest.ApiService.Application.Interfaces.IRepositories
{
    public interface IReportRepository
    {
        Task<Guid> SaveHourlyCityRevenueReportAsync(IEnumerable<HourlyCityRevenue> entries, DateOnly forDate);
        Task<ReportStatus> GetReportStatusAsync(Guid reportId);
        Task<IEnumerable<HourlyCityRevenue>> GetHourlyCityRevenueReportAsync(Guid reportId);
        Task<Guid> SaveVehicleCountByTollPlazaReportAsync(IEnumerable<VehicleCountByTollPlaza> entries, int tollPlazaId, DateOnly reportDate);
        Task<IEnumerable<VehicleCountByTollPlaza>> GetVehicleCountByTollPlazaReportAsync(int tollPlazaId, DateOnly reportDate);
    }
} 