using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TravelPricingGateway.Core;

namespace TravelPricingGateway.Infrastructure
{
    public class TraditionalXmlProvider : IHotelProvider
    {
        private const string ProviderName = nameof(TraditionalXmlProvider);

        [XmlRoot("HotelResponse")]
        public class XmlResponse
        {
            [XmlElement("Id")]
            public string Id { get; set; }

            [XmlElement("Cost")]
            public decimal Cost { get; set; }

            [XmlElement("Status")]
            public string Status { get; set; }
        }

        public async Task<HotelPriceResult> GetPriceAsync(HotelSearchRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                string xmlString = @"<HotelResponse><Id>HOTEL_123</Id><Cost>95.00</Cost><Status>Success</Status></HotelResponse>";

                await Task.Delay(300, cancellationToken);

                var serializer = new XmlSerializer(typeof(XmlResponse));
                using var reader = new StringReader(xmlString);

                var response = serializer.Deserialize(reader) as XmlResponse;

                if (response == null)
                    return HotelPriceResult.Failure(ProviderName);

                if (response.Status != "Success")
                    return HotelPriceResult.Failure(ProviderName);

                if (!string.Equals(response.Id, request.HotelId, StringComparison.OrdinalIgnoreCase))
                    return HotelPriceResult.Failure(ProviderName);

                return HotelPriceResult.Success(ProviderName, response.Cost);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{ProviderName} was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{ProviderName}] Failed with error: {ex.InnerException?.Message ?? ex.Message}");
                return HotelPriceResult.Failure(ProviderName);
            }
        }
    }
}