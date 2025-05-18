using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thunders.TechTest.ApiService.Data;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Models; // For TollUsageData and VehicleTypeData
using System;
using Microsoft.Extensions.Logging;

namespace Thunders.TechTest.ApiService.Services
{
    public interface IReportService
    {
        Task GenerateHourlyCityRevenueReportAsync(DateOnly forDate);
        Task GenerateTopEarningTollPlazasReportAsync(int year, int month, int topN);
        Task GenerateVehicleCountByTollPlazaReportAsync(string tollPlazaName, DateOnly forDate);
        Task<List<HourlyCityRevenue>> GetHourlyCityRevenueReportAsync(DateOnly forDate, string city, string state);
        Task<List<TopEarningTollPlaza>> GetTopEarningTollPlazasReportAsync(int year, int month, int count);
        Task<List<VehicleCountByTollPlaza>> GetVehicleCountByTollPlazaReportAsync(string tollPlazaName, DateOnly forDate);
    }

    public class ReportService : IReportService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ReportService> _logger;

        public ReportService(AppDbContext dbContext, ILogger<ReportService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task GenerateHourlyCityRevenueReportAsync(DateOnly forDate)
        {
            _logger.LogInformation("Generating Hourly City Revenue Report for {ForDate}", forDate);
            var hourlyData = await _dbContext.TollUsages
                .Where(t => DateOnly.FromDateTime(t.Timestamp) == forDate)
                .GroupBy(t => new { t.City, t.State, Hour = t.Timestamp.Hour })
                .Select(g => new HourlyCityRevenue
                {
                    ReportDate = forDate,
                    Hour = g.Key.Hour,
                    City = g.Key.City,
                    State = g.Key.State,
                    TotalAmount = g.Sum(t => t.AmountPaid)
                })
                .ToListAsync();

            if (hourlyData.Any())
            {
                // Clear old data for the same date to avoid duplicates if re-run
                var existingReports = await _dbContext.ReportHourlyCityRevenues
                    .Where(r => r.ReportDate == forDate)
                    .ToListAsync();
                if (existingReports.Any())
                {
                    _dbContext.ReportHourlyCityRevenues.RemoveRange(existingReports);
                }
                _dbContext.ReportHourlyCityRevenues.AddRange(hourlyData);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully generated {Count} records for Hourly City Revenue Report for {ForDate}", hourlyData.Count, forDate);
            }
            else
            {
                _logger.LogInformation("No data found to generate Hourly City Revenue Report for {ForDate}", forDate);
            }
        }

        public async Task GenerateTopEarningTollPlazasReportAsync(int year, int month, int topN)
        {
            _logger.LogInformation("Generating Top Earning Toll Plazas Report for {Year}-{Month}, Top {TopN}", year, month, topN);
            var topPlazas = await _dbContext.TollUsages
                .Where(t => t.Timestamp.Year == year && t.Timestamp.Month == month)
                .GroupBy(t => t.TollPlaza)
                .Select(g => new
                {
                    TollPlaza = g.Key,
                    TotalAmount = g.Sum(t => t.AmountPaid)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(topN)
                .ToListAsync();

            var reportEntries = topPlazas.Select((p, index) => new TopEarningTollPlaza
            {
                Year = year,
                Month = month,
                TollPlaza = p.TollPlaza,
                TotalAmount = p.TotalAmount,
                Rank = index + 1
            }).ToList();

            if (reportEntries.Any())
            {
                var existingReports = await _dbContext.ReportTopEarningTollPlazas
                    .Where(r => r.Year == year && r.Month == month)
                    .ToListAsync();
                if (existingReports.Any())
                {
                    _dbContext.ReportTopEarningTollPlazas.RemoveRange(existingReports);
                }
                _dbContext.ReportTopEarningTollPlazas.AddRange(reportEntries);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully generated {Count} records for Top Earning Toll Plazas Report for {Year}-{Month}", reportEntries.Count, year, month);
            }
            else
            {
                _logger.LogInformation("No data found to generate Top Earning Toll Plazas Report for {Year}-{Month}", year, month);
            }
        }

        public async Task GenerateVehicleCountByTollPlazaReportAsync(string tollPlazaName, DateOnly forDate)
        {
            _logger.LogInformation("Generating Vehicle Count by Toll Plaza Report for {TollPlazaName} on {ForDate}", tollPlazaName, forDate);
            var vehicleCounts = await _dbContext.TollUsages
                .Where(t => t.TollPlaza == tollPlazaName && DateOnly.FromDateTime(t.Timestamp) == forDate)
                .GroupBy(t => t.VehicleType)
                .Select(g => new VehicleCountByTollPlaza
                {
                    TollPlaza = tollPlazaName,
                    VehicleType = g.Key,
                    Count = g.Count(),
                    ReportDate = forDate
                })
                .ToListAsync();

            if (vehicleCounts.Any())
            {
                var existingReports = await _dbContext.ReportVehicleCountsByTollPlaza
                    .Where(r => r.TollPlaza == tollPlazaName && r.ReportDate == forDate)
                    .ToListAsync();
                if (existingReports.Any())
                {
                    _dbContext.ReportVehicleCountsByTollPlaza.RemoveRange(existingReports);
                }
                _dbContext.ReportVehicleCountsByTollPlaza.AddRange(vehicleCounts);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully generated {Count} records for Vehicle Count by Toll Plaza Report for {TollPlazaName} on {ForDate}", vehicleCounts.Count, tollPlazaName, forDate);
            }
            else
            {
                _logger.LogInformation("No data found to generate Vehicle Count by Toll Plaza Report for {TollPlazaName} on {ForDate}", tollPlazaName, forDate);
            }
        }

        public async Task<List<HourlyCityRevenue>> GetHourlyCityRevenueReportAsync(DateOnly forDate, string city, string state)
        {
            return await _dbContext.ReportHourlyCityRevenues
                .Where(r => r.ReportDate == forDate && r.City == city && r.State == state)
                .OrderBy(r => r.Hour)
                .ToListAsync();
        }

        public async Task<List<TopEarningTollPlaza>> GetTopEarningTollPlazasReportAsync(int year, int month, int count)
        {
            return await _dbContext.ReportTopEarningTollPlazas
                .Where(r => r.Year == year && r.Month == month)
                .OrderBy(r => r.Rank)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<VehicleCountByTollPlaza>> GetVehicleCountByTollPlazaReportAsync(string tollPlazaName, DateOnly forDate)
        {
            return await _dbContext.ReportVehicleCountsByTollPlaza
                .Where(r => r.TollPlaza == tollPlazaName && r.ReportDate == forDate)
                .OrderBy(r => r.VehicleType)
                .ToListAsync();
        }
    }
}