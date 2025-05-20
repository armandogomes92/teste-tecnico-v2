using System.Net.Http.Json;
using Thunders.TechTest.ApiService.Application.DTOs.Requests;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.Tests.Integrations;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__RabbitMq", "amqp://guest:guest@localhost:5672/");
        _factory = factory;
    }

    [Fact]
    public async Task TollUsageController_IngestData_ReturnsAccepted()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TollUsageDataRequest
        {
            Timestamp = DateTime.UtcNow,
            TollPlazaId = 1,
            City = CityEnum.SP_SaoPaulo,
            State = StateEnum.SP,
            AmountPaid = 10.5m,
            VehicleType = VehicleTypeEnum.Car
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/TollUsage/ingest", request);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }
} 