using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelPricingGateway.Core;

namespace TravelPricingGateway.Controllers
{
    [ApiController]
    [Route("api/hotels")]
    public class HotelPricingController : ControllerBase
    {
        private readonly HotelPricingAggregator _aggregator;
        private readonly IHotelRepository _hotelRepository;

        public HotelPricingController(HotelPricingAggregator aggregator, IHotelRepository hotelRepository)
        {
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        }

        [HttpGet("{hotelId}/cheapest-price")]
        public async Task<IActionResult> GetCheapestPrice(
            [FromRoute] string hotelId,
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                bool hotelExists = await _hotelRepository.HotelExistsAsync(hotelId, cancellationToken);
                if (!hotelExists)
                {
                    return NotFound(new { Message = $"Hotel '{hotelId}' was not found in our catalog." });
                }

                var stayPeriod = new DateRange(checkIn, checkOut);
                var request = new HotelSearchRequest(hotelId, stayPeriod, DateTime.UtcNow);

                var result = await _aggregator.GetCheapestPriceAsync(request, cancellationToken);

                if (result == null)
                {
                    return NotFound(new { Message = "No prices found for the requested dates or hotel." });
                }

                return Ok(result);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return StatusCode(StatusCodes.Status499ClientClosedRequest,
                    new { Error = "The request was canceled." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred while processing your request." });
            }
        }
    }
}