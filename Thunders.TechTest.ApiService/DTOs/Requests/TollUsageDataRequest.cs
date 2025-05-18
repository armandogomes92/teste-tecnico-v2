using System;
using System.ComponentModel.DataAnnotations;

namespace Thunders.TechTest.ApiService.DTOs.Requests
{
    public class TollUsageDataRequest
    {
        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [StringLength(100)]
        public string TollPlaza { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(2)] // Assuming UF (e.g., SP, RJ)
        public string State { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal AmountPaid { get; set; }

        [Required]
        public VehicleType VehicleType { get; set; }
    }

    public enum VehicleType
    {
        Motorcycle,
        Car,
        Truck
    }
}