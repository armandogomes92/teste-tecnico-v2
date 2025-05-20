namespace Thunders.TechTest.ApiService.Infrastructure.Messages;

public class TopEarningTollPlazasReportMessage
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int quantityOfPlazas { get; set; }
    public Guid ReportId { get; set; }
}