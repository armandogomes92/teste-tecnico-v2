using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;

namespace Thunders.TechTest.ApiService.Infrastructure.Data.Repositories;

public class ReportCacheRepository : IReportCacheRepository
{
    private readonly IDatabase _redis;
    public ReportCacheRepository(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _redis.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var json = await _redis.StringGetAsync(key);
            return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;

        }
        catch (Exception ex)
        {

            throw;
        }
    }
} 