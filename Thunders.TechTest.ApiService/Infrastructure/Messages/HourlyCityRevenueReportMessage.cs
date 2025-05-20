namespace Thunders.TechTest.ApiService.Infrastructure.Messages;

public class HourlyCityRevenueReportMessage
{
    public DateOnly ForDate { get; set; }
    public Guid ReportId { get; set; }
}