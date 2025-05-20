namespace Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
 
public interface IReportCacheRepository
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
} 