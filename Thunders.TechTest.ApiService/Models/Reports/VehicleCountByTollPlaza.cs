using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunders.TechTest.ApiService.Models.Reports
{
    [Table("ReportVehicleCountByTollPlaza")]
    public class VehicleCountByTollPlaza
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TollPlaza { get; set; }

        [Required]
        public VehicleTypeData VehicleType { get; set; } // Reusing the enum from TollUsageData

        [Required]
        public int Count { get; set; }

        [Required]
        public DateOnly ReportDate { get; set; } // Date for which the count is reported, could be a specific day or overall.
                                                // For simplicity, let's assume it's for a specific day the report is run.
    }
}