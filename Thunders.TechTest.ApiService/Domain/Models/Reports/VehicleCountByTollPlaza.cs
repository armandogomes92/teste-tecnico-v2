using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Domain.Models.Reports;

[Table("ReportVehicleCountByTollPlaza")]
public class VehicleCountByTollPlaza
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public int TollPlazaId { get; set; }

    [ForeignKey("TollPlazaId")]
    public TollPlaza TollPlaza { get; set; } = null!;

    [Required]
    public VehicleTypeEnum VehicleType { get; set; } 

    [Required]
    public int Count { get; set; }

    [Required]
    public DateOnly ReportDate { get; set; }

    [Required]
    public ReportStatus Status { get; set; }
}