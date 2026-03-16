using System.Threading;
using System.Threading.Tasks;

namespace TravelPricingGateway.Core
{
    public interface IHotelRepository
    {
        Task<bool> HotelExistsAsync(string hotelId, CancellationToken cancellationToken);
    }
}