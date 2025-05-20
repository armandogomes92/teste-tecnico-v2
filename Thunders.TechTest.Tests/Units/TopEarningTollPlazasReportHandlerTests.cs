using Moq;
using Thunders.TechTest.ApiService.Infrastructure.Messages.Handlers.Reports;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using Thunders.TechTest.ApiService.Domain.Enums;
using Thunders.TechTest.ApiService.Infrastructure.Messages;
using Thunders.TechTest.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Thunders.TechTest.Tests.Units;

public class TopEarningTollPlazasReportHandlerTests
{
    [Fact]
    public async Task Handle_Success_SetsStatusAndSavesReport()
    {
        // Arrange
        var mockCache = new Mock<IReportCacheRepository>();
        var mockLogger = new Mock<ILogger<TopEarningTollPlazasReportHandler>>();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        using var dbContext = new AppDbContext(options);
        var handler = new TopEarningTollPlazasReportHandler(mockCache.Object, mockLogger.Object, dbContext);
        var message = new TopEarningTollPlazasReportMessage { Year = 2024, Month = 6, quantityOfPlazas = 1, ReportId = Guid.NewGuid() };

        // Act
        await handler.Handle(message);

        // Assert
        mockCache.Verify(x => x.SetAsync<ReportStatus>(It.IsAny<string>(), ReportStatus.EmProcessamento, null), Times.Once);
        mockCache.Verify(x => x.SetAsync<object>(It.IsAny<string>(), It.IsAny<object>(), null), Times.AtLeastOnce);
        mockLogger.Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_Exception_SetsErrorStatusAndLogsError()
    {
        // Arrange
        var mockCache = new Mock<IReportCacheRepository>();
        var mockLogger = new Mock<ILogger<TopEarningTollPlazasReportHandler>>();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb2")
            .Options;
        using var dbContext = new AppDbContext(options);
        var handler = new TopEarningTollPlazasReportHandler(mockCache.Object, mockLogger.Object, dbContext);
        var message = new TopEarningTollPlazasReportMessage { Year = 2024, Month = 6, quantityOfPlazas = 1, ReportId = Guid.NewGuid() };
        mockCache.Setup(x => x.SetAsync<ReportStatus>(It.IsAny<string>(), ReportStatus.EmProcessamento, null)).ThrowsAsync(new Exception("Erro"));

        // Act
        await handler.Handle(message);

        // Assert
        mockCache.Verify(x => x.SetAsync<ReportStatus>(It.IsAny<string>(), ReportStatus.Erro, null), Times.Once);
        mockLogger.Verify(x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Error),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
    }
} 