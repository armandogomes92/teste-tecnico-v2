namespace Thunders.TechTest.ApiService.Infrastructure.Messages;

public class VehicleCountByTollPlazaReportMessage
{
    public int TollPlazaId { get; set; }
    public DateOnly ReportDate { get; set; }
    public Guid ReportId { get; set; }
}