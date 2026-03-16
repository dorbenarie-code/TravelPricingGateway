using System;

namespace TravelPricingGateway.Core
{
    public sealed record HotelSearchRequest
    {
        public string HotelId { get; }
        public DateRange StayPeriod { get; }

        public HotelSearchRequest(string hotelId, DateRange stayPeriod, DateTime currentDate)
        {
            if (string.IsNullOrWhiteSpace(hotelId))
                throw new ArgumentException("HotelId cannot be null, empty, or whitespace.", nameof(hotelId));

            if (stayPeriod == null)
                throw new ArgumentNullException(nameof(stayPeriod));

            var normalizedHotelId = hotelId.Trim();
            var referenceDate = currentDate.Date;

            if (stayPeriod.CheckIn < referenceDate)
                throw new ArgumentException("CheckIn date cannot be in the past.", nameof(stayPeriod));

            HotelId = normalizedHotelId;
            StayPeriod = stayPeriod;
        }
    }
}