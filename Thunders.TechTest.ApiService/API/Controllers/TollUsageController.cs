using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.ApiService.Application.DTOs.Requests;
using Thunders.TechTest.ApiService.Application.Interfaces.IServices;

namespace Thunders.TechTest.ApiService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TollUsageController : ControllerBase
{
    private readonly ITollUsageService _tollUsageService;

    public TollUsageController(ITollUsageService tollUsageService)
    {
        _tollUsageService = tollUsageService;
    }

    [HttpPost("ingest")]
    public async Task<IActionResult> IngestData([FromBody] TollUsageDataRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _tollUsageService.IngestDataAsync(request);

        return Accepted();
    }
}