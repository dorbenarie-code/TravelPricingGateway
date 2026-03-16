using System;

namespace TravelPricingGateway.Core
{
    public sealed record DateRange
    {
        public DateTime CheckIn { get; }
        public DateTime CheckOut { get; }

        public DateRange(DateTime checkIn, DateTime checkOut)
        {
            var normalizedCheckIn = checkIn.Date;
            var normalizedCheckOut = checkOut.Date;

            if (normalizedCheckOut <= normalizedCheckIn)
                throw new ArgumentException("CheckOut date must be strictly after CheckIn date.");

            CheckIn = normalizedCheckIn;
            CheckOut = normalizedCheckOut;
        }
    }
}