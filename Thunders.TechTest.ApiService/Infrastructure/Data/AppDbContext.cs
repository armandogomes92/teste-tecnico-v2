using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Domain.Models;
using Thunders.TechTest.ApiService.Domain.Models.Reports;

namespace Thunders.TechTest.ApiService.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TollUsageData> TollUsages { get; set; }
    public DbSet<TollPlaza> Plazas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TollUsageData>()
            .Property(u => u.AmountPaid)
            .HasPrecision(18, 2);
    }
}