using Thunders.TechTest.ApiService.Application.DTOs.Requests;
using Thunders.TechTest.ApiService.Application.Interfaces.IServices;
using Thunders.TechTest.ApiService.Domain.Models;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Application.Services;

public class TollUsageService : ITollUsageService
{
    private readonly IMessageSender _messageSender;

    public TollUsageService(IMessageSender messageSender)
    {
        _messageSender = messageSender;
    }

    public async Task IngestDataAsync(TollUsageDataRequest request)
    {
        var message = new TollUsageData
        {
            Timestamp = request.Timestamp,
            City = request.City,
            State = request.State,
            AmountPaid = request.AmountPaid,
            VehicleType = request.VehicleType,
            TollPlazaId = request.TollPlazaId
        };
        await _messageSender.SendLocal(message);
    }
}