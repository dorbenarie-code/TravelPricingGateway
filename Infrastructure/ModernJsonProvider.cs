using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TravelPricingGateway.Core;

namespace TravelPricingGateway.Infrastructure
{
    public class ModernJsonProvider : IHotelProvider
    {
        private const string ProviderName = nameof(ModernJsonProvider);

        private class JsonResponse
        {
            [JsonPropertyName("hotelId")]
            public string HotelId { get; set; }

            [JsonPropertyName("totalPrice")]
            public decimal TotalPrice { get; set; }

            [JsonPropertyName("responseStatus")]
            public string ResponseStatus { get; set; }
        }

        public async Task<HotelPriceResult> GetPriceAsync(HotelSearchRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                string jsonString = @"{ ""hotelId"": ""HOTEL_123"", ""totalPrice"": 115.50, ""responseStatus"": ""OK"" }";

                await Task.Delay(200, cancellationToken);

                var response = JsonSerializer.Deserialize<JsonResponse>(jsonString);

                if (response == null)
                    return HotelPriceResult.Failure(ProviderName);

                if (response.ResponseStatus != "OK")
                    return HotelPriceResult.Failure(ProviderName);

                if (!string.Equals(response.HotelId, request.HotelId, StringComparison.OrdinalIgnoreCase))
                    return HotelPriceResult.Failure(ProviderName);

                return HotelPriceResult.Success(ProviderName, response.TotalPrice);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{ProviderName} was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{ProviderName}] Failed with error: {ex.Message}");
                return HotelPriceResult.Failure(ProviderName);
            }
        }
    }
}