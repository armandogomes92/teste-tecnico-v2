using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.ApiService.Domain.Models;

namespace Thunders.TechTest.ApiService.Infrastructure.Data.Repositories;

public class TollUsageRepository : ITollUsageRepository
{
    private readonly AppDbContext _dbContext;
    public TollUsageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TollUsageData usageData)
    {
        _dbContext.TollUsages.Add(usageData);
        await _dbContext.SaveChangesAsync();
    }
}