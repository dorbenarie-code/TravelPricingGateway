using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TravelPricingGateway.Core
{
    public class HotelPricingAggregator
    {
        private readonly IHotelProvider[] _providers;

        public HotelPricingAggregator(IEnumerable<IHotelProvider> providers)
        {
            if (providers == null)
                throw new ArgumentNullException(nameof(providers));

            _providers = providers.ToArray();

            if (_providers.Any(provider => provider == null))
                throw new ArgumentException("Providers collection cannot contain null values.", nameof(providers));
        }

        public async Task<HotelPriceResult?> GetCheapestPriceAsync(
            HotelSearchRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            if (_providers.Length == 0)
                return null;

            var tasks = _providers
                .Select(provider => provider.GetPriceAsync(request, cancellationToken))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            return results
                .Where(result => result != null && result.IsSuccess)
                .MinBy(result => result.Price);
        }
    }
}