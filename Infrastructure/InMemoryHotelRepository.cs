using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; 
using TravelPricingGateway.Core;

namespace TravelPricingGateway.Infrastructure
{
    public class InMemoryHotelRepository : IHotelRepository
    {
        private readonly HashSet<string> _validHotels;

        public InMemoryHotelRepository(IConfiguration configuration)
        {
        
            var hotelsFromConfig = configuration.GetSection("ValidHotels").Get<string[]>() ?? Array.Empty<string>();

            _validHotels = new HashSet<string>(hotelsFromConfig, StringComparer.OrdinalIgnoreCase);
        }
        public Task<bool> HotelExistsAsync(string hotelId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(hotelId))
            {
                return Task.FromResult(false);
            }

            bool exists = _validHotels.Contains(hotelId);
            return Task.FromResult(exists);
        }
    }
}