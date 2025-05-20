using Xunit;
using Moq;
using System.Threading.Tasks;
using Thunders.TechTest.ApiService.Infrastructure.Messages.Handlers;
using Thunders.TechTest.ApiService.Domain.Models;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;

namespace Thunders.TechTest.Tests.Units;

public class TollUsageDataHandlerTests
{
    [Fact]
    public async Task Handle_Success_CallsAddAsyncAndLogsInformation()
    {
        // Arrange
        var mockRepo = new Mock<ITollUsageRepository>();
        var mockLogger = new Mock<ILogger<TollUsageDataHandler>>();
        var handler = new TollUsageDataHandler(mockRepo.Object, mockLogger.Object);
        var data = new TollUsageData {
            TollPlazaId = 1,
            City = 0,
            State = 0,
            AmountPaid = 10,
            VehicleType = 0,
            Timestamp = DateTime.UtcNow,
            TollPlaza = new TollPlaza { Id = 1, Name = "Praça Teste" }
        };

        // Act
        await handler.Handle(data);

        // Assert
        mockRepo.Verify(x => x.AddAsync(data), Times.Once);
        mockLogger.Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Exception_LogsError()
    {
        // Arrange
        var mockRepo = new Mock<ITollUsageRepository>();
        var mockLogger = new Mock<ILogger<TollUsageDataHandler>>();
        var handler = new TollUsageDataHandler(mockRepo.Object, mockLogger.Object);
        var data = new TollUsageData { TollPlazaId = 1, City = 0, State = 0, AmountPaid = 10, VehicleType = 0, Timestamp = DateTime.UtcNow, TollPlaza = null! };
        mockRepo.Setup(x => x.AddAsync(It.IsAny<TollUsageData>())).ThrowsAsync(new Exception("Erro"));

        // Act
        await handler.Handle(data);

        // Assert
        mockLogger.Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Error),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
    }
} 