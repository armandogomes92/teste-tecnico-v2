using Thunders.TechTest.ApiService.Domain.Models;

namespace Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;

public interface ITollUsageRepository
{
    Task AddAsync(TollUsageData usageData);
}