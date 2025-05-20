using System.ComponentModel.DataAnnotations;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Application.DTOs.Requests;

public class TollUsageDataRequest
{
    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    public int TollPlazaId { get; set; }

    [Required]
    public CityEnum City { get; set; }

    [Required]
    public StateEnum State { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal AmountPaid { get; set; }

    [Required]
    public VehicleTypeEnum VehicleType { get; set; }
}