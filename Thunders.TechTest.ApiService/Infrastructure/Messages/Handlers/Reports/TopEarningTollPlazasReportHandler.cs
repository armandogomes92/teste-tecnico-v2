using Rebus.Handlers;
using Thunders.TechTest.ApiService.Domain.Models.Reports;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.ApiService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Infrastructure.Data;

namespace Thunders.TechTest.ApiService.Infrastructure.Messages.Handlers.Reports;

public class TopEarningTollPlazasReportHandler : IHandleMessages<TopEarningTollPlazasReportMessage>
{
    private readonly IReportCacheRepository _cacheRepository;
    private readonly ILogger<TopEarningTollPlazasReportHandler> _logger;
    private readonly AppDbContext _dbContext;

    public TopEarningTollPlazasReportHandler(IReportCacheRepository cacheRepository, ILogger<TopEarningTollPlazasReportHandler> logger, AppDbContext dbContext)
    {
        _cacheRepository = cacheRepository;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(TopEarningTollPlazasReportMessage message)
    {
        try
        {
            // Status: EmProcessamento
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.EmProcessamento);

            _logger.LogInformation("Processando relatório das praças que mais faturaram para {Year}-{Month}, Top {quantityOfPlazas}", message.Year, message.Month, message.quantityOfPlazas);

            // Lógica real de agregação
            var startDate = new DateTime(message.Year, message.Month, 1);
            var endDate = startDate.AddMonths(1);
            var topPlazas = await _dbContext.TollUsages
                .Where(u => u.Timestamp >= startDate && u.Timestamp < endDate)
                .GroupBy(u => u.TollPlazaId)
                .Select(g => new {
                    TollPlazaId = g.Key,
                    TotalAmount = g.Sum(x => x.AmountPaid)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(message.quantityOfPlazas)
                .ToListAsync();

            var plazas = await _dbContext.Plazas.ToListAsync();
            var reportEntries = topPlazas.Select((x, idx) => new TopEarningTollPlaza {
                Year = message.Year,
                Month = message.Month,
                TollPlazaId = x.TollPlazaId,
                TollPlaza = plazas.FirstOrDefault(p => p.Id == x.TollPlazaId)!,
                TotalAmount = x.TotalAmount,
                Rank = idx + 1,
                Status = ReportStatus.Pronto
            }).ToList();

            // Salvar no Redis
            await _cacheRepository.SetAsync(message.ReportId.ToString(), reportEntries);
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.Pronto);
            _logger.LogInformation("Relatório salvo no Redis com sucesso para {Year}-{Month}, chave: {Key}", message.Year, message.Month, message.ReportId);
        }
        catch (Exception ex)
        {
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.Erro);
            _logger.LogError(ex, "Erro ao processar TopEarningTollPlazasReportMessage: {Erro}", ex.Message);
        }
    }
}