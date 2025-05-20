using Rebus.Handlers;
using Thunders.TechTest.ApiService.Domain.Models.Reports;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.ApiService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Infrastructure.Data;

namespace Thunders.TechTest.ApiService.Infrastructure.Messages.Handlers.Reports;

public class VehicleCountByTollPlazaReportHandler : IHandleMessages<VehicleCountByTollPlazaReportMessage>
{
    private readonly IReportCacheRepository _cacheRepository;
    private readonly ILogger<VehicleCountByTollPlazaReportHandler> _logger;
    private readonly AppDbContext _dbContext;

    public VehicleCountByTollPlazaReportHandler(IReportCacheRepository cacheRepository, ILogger<VehicleCountByTollPlazaReportHandler> logger, AppDbContext dbContext)
    {
        _cacheRepository = cacheRepository;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(VehicleCountByTollPlazaReportMessage message)
    {
        try
        {
            // Status: EmProcessamento
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.EmProcessamento);

            _logger.LogInformation("Processando relatório de contagem de tipos de veículos para praça {TollPlazaId} na data {ReportDate}", message.TollPlazaId, message.ReportDate);

            // Lógica real de agregação
            var vehicleCounts = await _dbContext.TollUsages
                .Where(u => u.TollPlazaId == message.TollPlazaId && DateOnly.FromDateTime(u.Timestamp) == message.ReportDate)
                .GroupBy(u => u.VehicleType)
                .Select(g => new VehicleCountByTollPlaza
                {
                    TollPlazaId = message.TollPlazaId,
                    TollPlaza = _dbContext.Plazas.FirstOrDefault(p => p.Id == message.TollPlazaId)!,
                    VehicleType = g.Key,
                    Count = g.Count(),
                    ReportDate = message.ReportDate,
                    Status = ReportStatus.Pronto
                })
                .ToListAsync();

            // Salvar no Redis
            await _cacheRepository.SetAsync(message.ReportId.ToString(), vehicleCounts);
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.Pronto);
            _logger.LogInformation("Relatório salvo no Redis com sucesso para praça {TollPlazaId} na data {ReportDate}, chave: {Key}", message.TollPlazaId, message.ReportDate, message.ReportId);
        }
        catch (Exception ex)
        {
            await _cacheRepository.SetAsync($"status:{message.ReportId}", ReportStatus.Erro);
            _logger.LogError(ex, "Erro ao processar VehicleCountByTollPlazaReportMessage: {Erro}", ex.Message);
        }
    }
}