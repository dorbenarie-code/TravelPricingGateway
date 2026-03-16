using System;

namespace TravelPricingGateway.Core
{
    public sealed record HotelPriceResult
    {
        public decimal Price { get; }
        public string ProviderName { get; }
        public bool IsSuccess { get; }

        private HotelPriceResult(decimal price, string providerName, bool isSuccess)
        {
            Price = price;
            ProviderName = providerName;
            IsSuccess = isSuccess;
        }

        public static HotelPriceResult Success(string providerName, decimal price)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException("Provider name must be provided.", nameof(providerName));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(price));

            var normalizedProviderName = providerName.Trim();

            return new HotelPriceResult(price, normalizedProviderName, true);
        }

        public static HotelPriceResult Failure(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException("Provider name must be provided.", nameof(providerName));

            var normalizedProviderName = providerName.Trim();

            return new HotelPriceResult(0m, normalizedProviderName, false);
        }
    }
}