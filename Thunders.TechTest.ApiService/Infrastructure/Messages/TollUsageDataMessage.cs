using Thunders.TechTest.ApiService.Domain.Enums;
using Thunders.TechTest.ApiService.Domain.Models;

namespace Thunders.TechTest.ApiService.Infrastructure.Messages;

public class TollUsageDataMessage
{
    public DateTime Timestamp { get; set; }
    public TollPlaza TollPlaza { get; set; } = null!;
    public CityEnum City { get; set; }
    public StateEnum State { get; set; }
    public decimal AmountPaid { get; set; }
    public VehicleTypeEnum VehicleType { get; set; }
}