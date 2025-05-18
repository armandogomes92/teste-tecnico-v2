using System;

namespace Thunders.TechTest.ApiService.Messages
{
    public class TollUsageDataMessage
    {
        public DateTime Timestamp { get; set; }
        public string TollPlaza { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal AmountPaid { get; set; }
        public VehicleTypeMessage VehicleType { get; set; }
    }

    public enum VehicleTypeMessage
    {
        Motorcycle,
        Car,
        Truck
    }
}