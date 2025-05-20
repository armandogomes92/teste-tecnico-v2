using System.Diagnostics;

namespace Thunders.TechTest.ApiService
{
    public static class Telemetry
    {
        public static readonly ActivitySource Source = new("Thunders.TechTest.ApiService");
    }
} 