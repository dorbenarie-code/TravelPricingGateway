using System;

namespace TravelPricingGateway.Core
{
    public sealed record DateRange
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        private DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static bool TryCreate(DateTime start, DateTime end, out DateRange dateRange)
        {
            var normalizedStart = start.Date;
            var normalizedEnd = end.Date;

            if (normalizedEnd <= normalizedStart)
            {
                dateRange = null;
                return false;
            }

            dateRange = new DateRange(normalizedStart, normalizedEnd);
            return true;
        }
    }
}