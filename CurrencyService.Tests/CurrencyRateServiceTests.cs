using CurrencyService.Database.Repositories;
using CurrencyService.HttpClients;
using CurrencyService.Models;
using CurrencyService.Services.Impl;
using Moq;

namespace CurrencyService.Tests;

public class CurrencyRateServiceTests
{
    [Fact]
    public async Task SyncRatesAsync_ShouldFetchAndInsert_WhenDatesAreValid()
    {
        // Arrange
        var latestDate = DateTime.UtcNow.Date.AddDays(-1);
        var expectedStart = latestDate.AddDays(1);
        var expectedRates = new List<CurrencyRate> {
            new() { CurrencyCode = "USD", Date = expectedStart, Rate = 38.5m }
        };

        var repoMock = new Mock<ICurrencyRateRepository>();
        repoMock.Setup(r => r.GetLatestDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(latestDate);

        var apiMock = new Mock<ICurrencyRateApiClient>();
        apiMock.Setup(api => api.GetCurrencyRatesAsync(expectedStart, null))
            .ReturnsAsync(expectedRates);

        var service = new CurrencyRateService(repoMock.Object, apiMock.Object);

        // Act
        await service.SyncRatesAsync(CancellationToken.None);

        // Assert
        repoMock.Verify(r => r.AddAsync(expectedRates, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task SyncRatesAsync_ShouldNotAdd_WhenAllRatesAreUpToDate()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var expectedStart = today.AddDays(1);
        
        var repoMock = new Mock<ICurrencyRateRepository>();
        repoMock.Setup(r => r.GetLatestDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(today);

        var apiMock = new Mock<ICurrencyRateApiClient>();
        apiMock.Setup(api => api.GetCurrencyRatesAsync(expectedStart, null))
            .ReturnsAsync([]);
        
        var service = new CurrencyRateService(repoMock.Object, apiMock.Object);

        // Act
        await service.SyncRatesAsync(CancellationToken.None);

        // Assert
        apiMock.Verify(api => api.GetCurrencyRatesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        repoMock.Verify(r => r.AddAsync(It.IsAny<IEnumerable<CurrencyRate>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task SyncRatesAsync_ShouldUseDefaultStartDate_WhenNoLatestDate()
    {
        // Arrange
        var defaultStart = DateTime.UtcNow.Date.AddMonths(-3);
        var expectedRates = new List<CurrencyRate> {
            new() { CurrencyCode = "USD", Date = defaultStart, Rate = 39.9m }
        };

        var repoMock = new Mock<ICurrencyRateRepository>();
        repoMock.Setup(r => r.GetLatestDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime?)null);

        var apiMock = new Mock<ICurrencyRateApiClient>();
        apiMock.Setup(api => api.GetCurrencyRatesAsync(defaultStart, null))
            .ReturnsAsync(expectedRates);

        var service = new CurrencyRateService(repoMock.Object, apiMock.Object);

        // Act
        await service.SyncRatesAsync(CancellationToken.None);

        // Assert
        repoMock.Verify(r => r.AddAsync(expectedRates, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task SyncRatesAsync_ShouldStartFromThreeMonthsAgo_WhenNoLatestDateExists()
    {
        // Arrange
        var expectedStart = DateTime.UtcNow.Date.AddMonths(-3);
        var expectedRates = new List<CurrencyRate> {
            new() { CurrencyCode = "USD", Date = expectedStart, Rate = 41m }
        };
        
        var repoMock = new Mock<ICurrencyRateRepository>();
        repoMock
            .Setup(r => r.GetLatestDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime?)null);

        var apiMock = new Mock<ICurrencyRateApiClient>();
        apiMock
            .Setup(c => c.GetCurrencyRatesAsync(It.IsAny<DateTime>(), null))
            .ReturnsAsync(expectedRates);

        var service = new CurrencyRateService(repoMock.Object, apiMock.Object);

        // Act
        await service.SyncRatesAsync(CancellationToken.None);

        // Assert
        apiMock.Verify(c => c.GetCurrencyRatesAsync(
            It.Is<DateTime?>(start => start != null && start.Value == expectedStart), null
        ));
    }
}
