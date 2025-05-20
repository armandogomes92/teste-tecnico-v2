using Thunders.TechTest.ApiService.Application.DTOs.Requests;

namespace Thunders.TechTest.ApiService.Application.Interfaces.IServices;

public interface ITollUsageService
{
    Task IngestDataAsync(TollUsageDataRequest request);
}