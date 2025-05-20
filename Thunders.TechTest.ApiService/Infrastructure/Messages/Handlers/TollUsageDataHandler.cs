using Rebus.Handlers;
using Thunders.TechTest.ApiService.Domain.Models;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;

namespace Thunders.TechTest.ApiService.Infrastructure.Messages.Handlers;

public class TollUsageDataHandler : IHandleMessages<TollUsageData>
{
    private readonly ITollUsageRepository _usageRepository;
    private readonly ILogger<TollUsageDataHandler> _logger;

    public TollUsageDataHandler(ITollUsageRepository usageRepository, ILogger<TollUsageDataHandler> logger)
    {
        _usageRepository = usageRepository;
        _logger = logger;
    }

    public async Task Handle(TollUsageData tollUsageData)
    {
        try
        {
            await _usageRepository.AddAsync(tollUsageData);
            _logger.LogInformation("Toll usage salvo com sucesso para cidade {City}, estado {State}",  tollUsageData.City, tollUsageData.State);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar TollUsageDataRequest: {Erro}", ex.Message);
        }
    }
}