using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Domain.Models.Reports;

[Table("ReportHourlyCityRevenue")]
public class HourlyCityRevenue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public DateOnly ReportDate { get; set; } // YYYY-MM-DD

    [Required]
    public int Hour { get; set; } // 0-23

    [Required]
    public CityEnum City { get; set; }

    [Required]
    public StateEnum State { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public ReportStatus Status { get; set; }
    public Guid ReportId { get; internal set; }
}