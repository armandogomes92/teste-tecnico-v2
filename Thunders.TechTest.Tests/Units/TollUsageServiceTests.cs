using Xunit;
using Moq;
using System.Threading.Tasks;
using Thunders.TechTest.ApiService.Application.Services;
using Thunders.TechTest.ApiService.Application.DTOs.Requests;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.Tests.Units;

public class TollUsageServiceTests
{
    [Fact]
    public async Task IngestDataAsync_Success_CallsMessageSender()
    {
        // Arrange
        var mockSender = new Mock<IMessageSender>();
        var service = new TollUsageService(mockSender.Object);
        var request = new TollUsageDataRequest
        {
            Timestamp = DateTime.UtcNow,
            TollPlazaId = 1,
            City = 0,
            State = 0,
            AmountPaid = 10.5m,
            VehicleType = 0
        };

        // Act
        await service.IngestDataAsync(request);

        // Assert
        mockSender.Verify(x => x.SendLocal(request), Times.Once);
    }

    [Fact]
    public async Task IngestDataAsync_Exception_Throws()
    {
        // Arrange
        var mockSender = new Mock<IMessageSender>();
        var service = new TollUsageService(mockSender.Object);
        var request = new TollUsageDataRequest();
        mockSender.Setup(x => x.SendLocal(It.IsAny<TollUsageDataRequest>())).ThrowsAsync(new Exception("Erro"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.IngestDataAsync(request));
    }
} 