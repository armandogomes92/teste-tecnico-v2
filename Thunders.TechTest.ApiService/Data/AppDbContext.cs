using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TollUsageData> TollUsages { get; set; }
        public DbSet<Thunders.TechTest.ApiService.Models.Reports.HourlyCityRevenue> ReportHourlyCityRevenues { get; set; }
        public DbSet<Thunders.TechTest.ApiService.Models.Reports.TopEarningTollPlaza> ReportTopEarningTollPlazas { get; set; }
        public DbSet<Thunders.TechTest.ApiService.Models.Reports.VehicleCountByTollPlaza> ReportVehicleCountsByTollPlaza { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entities here if needed
            // Example: modelBuilder.Entity<TollUsageData>().ToTable("TollUsageData");
            // This is already handled by the [Table] attribute in the TollUsageData model
        }
    }
}