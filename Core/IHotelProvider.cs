using System.Threading;
using System.Threading.Tasks;

namespace TravelPricingGateway.Core
{
    public interface IHotelProvider
    {
        Task<HotelPriceResult> GetPriceAsync(HotelSearchRequest request, CancellationToken cancellationToken);
    }
}