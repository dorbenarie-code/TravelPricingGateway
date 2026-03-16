using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using TravelPricingGateway.Core;

namespace TravelPricingGateway.Tests
{
    public class HotelPricingAggregatorTests
    {
        private static HotelSearchRequest CreateValidRequest()
        {
            var currentDate = new DateTime(2026, 3, 16);
            var stayPeriod = new DateRange(
                currentDate.AddDays(1),
                currentDate.AddDays(2));

            return new HotelSearchRequest("HOTEL_123", stayPeriod, currentDate);
        }

        [Fact]
        public async Task GetCheapestPriceAsync_TwoSuccessfulProviders_ReturnsCheapestPrice()
        {
            var request = CreateValidRequest();

            var expensiveProviderMock = new Mock<IHotelProvider>();
            expensiveProviderMock
                .Setup(p => p.GetPriceAsync(It.IsAny<HotelSearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(HotelPriceResult.Success("ExpensiveProvider", 150m));

            var cheapProviderMock = new Mock<IHotelProvider>();
            cheapProviderMock
                .Setup(p => p.GetPriceAsync(It.IsAny<HotelSearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(HotelPriceResult.Success("CheapProvider", 95m));

            var providers = new List<IHotelProvider>
            {
                expensiveProviderMock.Object,
                cheapProviderMock.Object
            };

            var aggregator = new HotelPricingAggregator(providers);

            var result = await aggregator.GetCheapestPriceAsync(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(95m, result.Price);
            Assert.Equal("CheapProvider", result.ProviderName);
        }

        [Fact]
        public async Task GetCheapestPriceAsync_OneProviderFails_ReturnsSuccessfulProviderPrice()
        {
            var request = CreateValidRequest();

            var successfulProviderMock = new Mock<IHotelProvider>();
            successfulProviderMock
                .Setup(p => p.GetPriceAsync(It.IsAny<HotelSearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(HotelPriceResult.Success("WorkingProvider", 150m));

            var failingProviderMock = new Mock<IHotelProvider>();
            failingProviderMock
                .Setup(p => p.GetPriceAsync(It.IsAny<HotelSearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(HotelPriceResult.Failure("FailingProvider"));

            var providers = new List<IHotelProvider>
            {
                successfulProviderMock.Object,
                failingProviderMock.Object
            };

            var aggregator = new HotelPricingAggregator(providers);

            var result = await aggregator.GetCheapestPriceAsync(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(150m, result.Price);
            Assert.Equal("WorkingProvider", result.ProviderName);
        }

        [Fact]
        public async Task GetCheapestPriceAsync_AllProvidersFail_ReturnsNull()
        {
            var request = CreateValidRequest();

            var failingProvider1 = new Mock<IHotelProvider>();
            failingProvider1
                .Setup(p => p.GetPriceAsync(It.IsAny<HotelSearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(HotelPriceResult.Failure("XmlProvider"));

            var failingProvider2 = new Mock<IHotelProvider>();
            failingProvider2
                .Setup(p => p.GetPriceAsync(It.IsAny<HotelSearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(HotelPriceResult.Failure("JsonProvider"));

            var providers = new List<IHotelProvider>
            {
                failingProvider1.Object,
                failingProvider2.Object
            };

            var aggregator = new HotelPricingAggregator(providers);

            var result = await aggregator.GetCheapestPriceAsync(request, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCheapestPriceAsync_CancellationRequested_ThrowsOperationCanceledException()
        {
            var request = CreateValidRequest();
            var aggregator = new HotelPricingAggregator(Array.Empty<IHotelProvider>());

            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                aggregator.GetCheapestPriceAsync(request, cancellationTokenSource.Token));
        }
    }
}