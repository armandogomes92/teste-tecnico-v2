using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunders.TechTest.ApiService.Models
{
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
        public string TollPlaza { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(2)]
        public string State { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }

        [Required]
        public VehicleTypeData VehicleType { get; set; }
    }

    public enum VehicleTypeData
    {
        Motorcycle,
        Car,
        Truck
    }
}