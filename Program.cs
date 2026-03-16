using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TravelPricingGateway.Core;
using TravelPricingGateway.Infrastructure;

namespace TravelPricingGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers(); 
            builder.Services.AddSingleton<IHotelRepository, InMemoryHotelRepository>();

            builder.Services.AddTransient<IHotelProvider, ModernJsonProvider>();
            builder.Services.AddTransient<IHotelProvider, TraditionalXmlProvider>();
            
            builder.Services.AddTransient<HotelPricingAggregator>();

            var app = builder.Build();

            app.MapControllers();
            app.Run("http://localhost:5005");
        }
    }
}