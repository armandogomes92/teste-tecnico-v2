using Rebus.Handlers;
using Thunders.TechTest.ApiService.Domain.Models.Reports;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.ApiService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Infrastructure.Data;

namespace Thunders.TechTest.ApiService.Infrastructure.Messages.Handlers.Reports;

public class HourlyCityRevenueReportHandler : IHandleMessages<HourlyCityRevenueReportMessage>
{
    private readonly IReportCacheRepository _cacheRepository;
    private readonly ILogger<HourlyCityRevenueReportHandler> _logger;
    private readonly AppDbContext _dbContext;

    public HourlyCityRevenueReportHandler(IReportCacheRepository cacheRepository, ILogger<HourlyCityRevenueReportHandler> logger, AppDbContext dbContext)
    {
        _cacheRepository = cacheRepository;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(HourlyCityRevenueReportMessage message)
    {
        using var activity = Telemetry.Source.StartActivity("HandleHourlyCityRevenueReport");
        activity?.SetTag("report.forDate", message.ForDate.ToString());
        try
        {
            // Status: EmProcessamento
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.EmProcessamento);

            var forDate = message.ForDate;
            _logger.LogInformation("Processando relatório de valor total por hora por cidade para {ForDate}", forDate);

            // Lógica real de agregação
            var hourlyData = await _dbContext.TollUsages
                .Where(u => DateOnly.FromDateTime(u.Timestamp) == forDate)
                .GroupBy(u => new { u.City, u.State, Hour = u.Timestamp.Hour })
                .Select(g => new HourlyCityRevenue
                {
                    ReportDate = forDate,
                    Hour = g.Key.Hour,
                    City = g.Key.City,
                    State = g.Key.State,
                    TotalAmount = g.Sum(x => x.AmountPaid),
                    Status = ReportStatus.Pronto,
                    ReportId = message.ReportId
                })
                .ToListAsync();

            // Salvar no Redis
            await _cacheRepository.SetAsync(message.ReportId.ToString(), hourlyData);
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.Pronto);
            _logger.LogInformation("Relatório salvo no Redis com sucesso para {ForDate}, chave: {Key}", forDate, message.ReportId);
            activity?.SetTag("report.status", "Pronto");
        }
        catch (Exception ex)
        {
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.Erro);
            _logger.LogError(ex, "Erro ao processar HourlyCityRevenueReportMessage: {Erro}", ex.Message);
            activity?.SetTag("report.status", "Erro");
        }
    }
}