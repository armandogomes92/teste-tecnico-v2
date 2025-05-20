using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Domain.Models;

[Table("TollUsageData")]
public class TollUsageData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    [MaxLength(100)]
    public CityEnum City { get; set; }

    [Required]
    [MaxLength(2)]
    public StateEnum State { get; set; }

    [Required]
    public decimal AmountPaid { get; set; }

    [Required]
    public VehicleTypeEnum VehicleType { get; set; }

    [Required]
    [ForeignKey("TollPlazaId")]
    public int TollPlazaId { get; set; }

    public TollPlaza TollPlaza { get; set; } = null!;
}