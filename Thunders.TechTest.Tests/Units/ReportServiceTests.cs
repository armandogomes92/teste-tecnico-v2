using Moq;
using Thunders.TechTest.ApiService.Application.Interfaces.IServices;
using Thunders.TechTest.ApiService.Application.Services;
using Thunders.TechTest.ApiService.Domain.Models.Reports;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.OutOfBox.Queues;
using Microsoft.Extensions.Logging;
using Thunders.TechTest.ApiService.Infrastructure.Messages;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.Tests.Units;

public class ReportServiceTests
{
    private readonly Mock<IMessageSender> _messageSenderMock;
    private readonly Mock<IReportCacheRepository> _cacheRepositoryMock;
    private readonly Mock<ILogger<ReportService>> _loggerMock;
    private readonly IReportService _reportService;

    public ReportServiceTests()
    {
        _messageSenderMock = new Mock<IMessageSender>();
        _cacheRepositoryMock = new Mock<IReportCacheRepository>();
        _loggerMock = new Mock<ILogger<ReportService>>();
        _reportService = new ReportService(_loggerMock.Object, _messageSenderMock.Object, _cacheRepositoryMock.Object);
    }

    [Fact]
    public async Task GenerateHourlyCityRevenueReportAsync_ShouldSendMesssageAndReturnReportId()
    {
        // Arrange
        var forDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _messageSenderMock.Setup(x => x.SendLocal(It.IsAny<object>())).Returns(Task.CompletedTask);

        // Act
        var reportId = await _reportService.GenerateHourlyCityRevenueReportAsync(forDate);

        // Assert
        Assert.NotEqual(Guid.Empty, reportId);
        _messageSenderMock.Verify(x => x.SendLocal(It.Is<HourlyCityRevenueReportMessage>(m => m.ForDate == forDate && m.ReportId == reportId)), Times.Once);
    }
    
    [Fact]
    public async Task GenerateTopEarningTollPlazasReportAsync_ShouldSendMessageAndReturnReportId()
    {
        // Arrange
        var year = 2024;
        var month = 7;
        var quantityOfPlazas = 10;
        _messageSenderMock.Setup(x => x.SendLocal(It.IsAny<object>())).Returns(Task.CompletedTask);

        // Act
        var reportId = await _reportService.GenerateTopEarningTollPlazasReportAsync(year, month, quantityOfPlazas);

        // Assert
        Assert.NotEqual(Guid.Empty, reportId);
        _messageSenderMock.Verify(x => x.SendLocal(It.Is<TopEarningTollPlazasReportMessage>(m => m.Year == year && m.Month == month && m.quantityOfPlazas == quantityOfPlazas && m.ReportId == reportId)), Times.Once);
    }

    [Fact]
    public async Task GenerateVehicleCountByTollPlazaReportAsync_ShouldSendMessageAndReturnReportId()
    {
        // Arrange
        var tollPlazaId = 1;
        var reportDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _messageSenderMock.Setup(x => x.SendLocal(It.IsAny<object>())).Returns(Task.CompletedTask);
        // Act
        var reportId = await _reportService.GenerateVehicleCountByTollPlazaReportAsync(tollPlazaId, reportDate);

        // Assert
        Assert.NotEqual(Guid.Empty, reportId);
        _messageSenderMock.Verify(x => x.SendLocal(It.Is<VehicleCountByTollPlazaReportMessage>(m => m.TollPlazaId == tollPlazaId && m.ReportDate == reportDate && m.ReportId == reportId)), Times.Once);
    }
    
    [Fact]
    public async Task GetHourlyCityRevenueReportStatusAsync_ShouldReturnStatusFromCache()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedStatus = ReportStatus.Pronto;
        _cacheRepositoryMock.Setup(x => x.GetAsync<ReportStatus>($"status:{reportId}")).ReturnsAsync(expectedStatus);

        // Act
        var status = await _reportService.GetHourlyCityRevenueReportStatusAsync(reportId);

        // Assert
        Assert.Equal(expectedStatus, status);
        _cacheRepositoryMock.Verify(x => x.GetAsync<ReportStatus>($"status:{reportId}"), Times.Once);
    }

    [Fact]
    public async Task GetHourlyCityRevenueReportResultAsync_ShouldReturnResultFromCache_WhenDataExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedResult = new List<HourlyCityRevenue> 
        { 
            new HourlyCityRevenue 
            { 
                Id = 1,
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Hour = 10,
                City = CityEnum.SP_SaoPaulo,
                State = StateEnum.SP,
                TotalAmount = 150.75m,
            },
            new HourlyCityRevenue 
            { 
                Id = 2,
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Hour = 11,
                City = CityEnum.SP_SaoPaulo,
                State = StateEnum.SP,
                TotalAmount = 200.00m,
            } 
        };
        _cacheRepositoryMock.Setup(x => x.GetAsync<List<HourlyCityRevenue>>(reportId.ToString())).ReturnsAsync(expectedResult);

        // Act
        var result = await _reportService.GetHourlyCityRevenueReportResultAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Count, result.Count());
        Assert.Equal(expectedResult, result);
        for(int i = 0; i < expectedResult.Count; i++)
        {
            Assert.Equal(expectedResult[i].Id, result.ElementAt(i).Id);
            Assert.Equal(expectedResult[i].ReportId, result.ElementAt(i).ReportId);
            Assert.Equal(expectedResult[i].ReportDate, result.ElementAt(i).ReportDate);
            Assert.Equal(expectedResult[i].Hour, result.ElementAt(i).Hour);
            Assert.Equal(expectedResult[i].City, result.ElementAt(i).City);
            Assert.Equal(expectedResult[i].State, result.ElementAt(i).State);
            Assert.Equal(expectedResult[i].TotalAmount, result.ElementAt(i).TotalAmount);
            Assert.Equal(expectedResult[i].Status, result.ElementAt(i).Status);
        }
        _cacheRepositoryMock.Verify(x => x.GetAsync<List<HourlyCityRevenue>>(reportId.ToString()), Times.Once);
    }

    [Fact]
    public async Task GetHourlyCityRevenueReportResultAsync_ShouldReturnEmptyList_WhenDataNotExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _cacheRepositoryMock.Setup(x => x.GetAsync<List<HourlyCityRevenue>>(reportId.ToString())).ReturnsAsync((List<HourlyCityRevenue>)null);

        // Act
        var result = await _reportService.GetHourlyCityRevenueReportResultAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _cacheRepositoryMock.Verify(x => x.GetAsync<List<HourlyCityRevenue>>(reportId.ToString()), Times.Once);
    }
    
     [Fact]
    public async Task GetTopEarningTollPlazasReportStatusAsync_ShouldReturnStatusFromCache()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedStatus = ReportStatus.EmProcessamento;
        _cacheRepositoryMock.Setup(x => x.GetAsync<ReportStatus>($"status:{reportId}")).ReturnsAsync(expectedStatus);

        // Act
        var status = await _reportService.GetTopEarningTollPlazasReportStatusAsync(reportId);

        // Assert
        Assert.Equal(expectedStatus, status);
        _cacheRepositoryMock.Verify(x => x.GetAsync<ReportStatus>($"status:{reportId}"), Times.Once);
    }

    [Fact]
    public async Task GetTopEarningTollPlazasReportResultAsync_ShouldReturnResultFromCache_WhenDataExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedResult = new List<TopEarningTollPlaza> 
        { 
            new TopEarningTollPlaza 
            { 
                Id = 1,
                Year = 2024,
                Month = 7,
                TollPlazaId = 101,
                TotalAmount = 5000.00m,
                Rank = 1,
                Status = ReportStatus.Pronto
            },
            new TopEarningTollPlaza 
            { 
                Id = 2,
                Year = 2024,
                Month = 7,
                TollPlazaId = 102,
                TotalAmount = 4500.00m,
                Rank = 2,
                Status = ReportStatus.Pronto
            }
        };
         _cacheRepositoryMock.Setup(x => x.GetAsync<List<TopEarningTollPlaza>>(reportId.ToString())).ReturnsAsync(expectedResult);

        // Act
        var result = await _reportService.GetTopEarningTollPlazasReportResultAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Count, result.Count());
        Assert.Equal(expectedResult, result);
        for(int i = 0; i < expectedResult.Count; i++)
        {
            Assert.Equal(expectedResult[i].Id, result.ElementAt(i).Id);
            Assert.Equal(expectedResult[i].Year, result.ElementAt(i).Year);
            Assert.Equal(expectedResult[i].Month, result.ElementAt(i).Month);
            Assert.Equal(expectedResult[i].TollPlazaId, result.ElementAt(i).TollPlazaId);
            Assert.Equal(expectedResult[i].TotalAmount, result.ElementAt(i).TotalAmount);
            Assert.Equal(expectedResult[i].Rank, result.ElementAt(i).Rank);
            Assert.Equal(expectedResult[i].Status, result.ElementAt(i).Status);
        }
        _cacheRepositoryMock.Verify(x => x.GetAsync<List<TopEarningTollPlaza>>(reportId.ToString()), Times.Once);
    }

    [Fact]
    public async Task GetTopEarningTollPlazasReportResultAsync_ShouldReturnEmptyList_WhenDataNotExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _cacheRepositoryMock.Setup(x => x.GetAsync<List<TopEarningTollPlaza>>(reportId.ToString())).ReturnsAsync((List<TopEarningTollPlaza>)null);

        // Act
        var result = await _reportService.GetTopEarningTollPlazasReportResultAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _cacheRepositoryMock.Verify(x => x.GetAsync<List<TopEarningTollPlaza>>(reportId.ToString()), Times.Once);
    }

    [Fact]
    public async Task GetVehicleCountByTollPlazaReportStatusAsync_ShouldReturnStatusFromCache()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedStatus = ReportStatus.Erro;
        _cacheRepositoryMock.Setup(x => x.GetAsync<ReportStatus>($"status:{reportId}")).ReturnsAsync(expectedStatus);

        // Act
        var status = await _reportService.GetVehicleCountByTollPlazaReportStatusAsync(reportId);

        // Assert
        Assert.Equal(expectedStatus, status);
        _cacheRepositoryMock.Verify(x => x.GetAsync<ReportStatus>($"status:{reportId}"), Times.Once);
    }

    [Fact]
    public async Task GetVehicleCountByTollPlazaReportResultAsync_ShouldReturnResultFromCache_WhenDataExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedResult = new List<VehicleCountByTollPlaza> 
        { 
            new VehicleCountByTollPlaza 
            {
                Id = 1,
                TollPlazaId = 201,
                VehicleType = VehicleTypeEnum.Car,
                Count = 150,
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = ReportStatus.Pronto
            },
            new VehicleCountByTollPlaza
            {
                Id = 2,
                TollPlazaId = 201,
                VehicleType = VehicleTypeEnum.Truck,
                Count = 50,
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = ReportStatus.Pronto
            }
        };
        _cacheRepositoryMock.Setup(x => x.GetAsync<List<VehicleCountByTollPlaza>>(reportId.ToString())).ReturnsAsync(expectedResult);

        // Act
        var result = await _reportService.GetVehicleCountByTollPlazaReportResultAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Count, result.Count());
        Assert.Equal(expectedResult, result);
         for(int i = 0; i < expectedResult.Count; i++)
        {
            Assert.Equal(expectedResult[i].Id, result.ElementAt(i).Id);
            Assert.Equal(expectedResult[i].TollPlazaId, result.ElementAt(i).TollPlazaId);
            Assert.Equal(expectedResult[i].VehicleType, result.ElementAt(i).VehicleType);
            Assert.Equal(expectedResult[i].Count, result.ElementAt(i).Count);
            Assert.Equal(expectedResult[i].ReportDate, result.ElementAt(i).ReportDate);
            Assert.Equal(expectedResult[i].Status, result.ElementAt(i).Status);
        }
        _cacheRepositoryMock.Verify(x => x.GetAsync<List<VehicleCountByTollPlaza>>(reportId.ToString()), Times.Once);
    }

    [Fact]
    public async Task GetVehicleCountByTollPlazaReportResultAsync_ShouldReturnEmptyList_WhenDataNotExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _cacheRepositoryMock.Setup(x => x.GetAsync<List<VehicleCountByTollPlaza>>(reportId.ToString())).ReturnsAsync((List<VehicleCountByTollPlaza>)null);

        // Act
        var result = await _reportService.GetVehicleCountByTollPlazaReportResultAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _cacheRepositoryMock.Verify(x => x.GetAsync<List<VehicleCountByTollPlaza>>(reportId.ToString()), Times.Once);
    }
} 