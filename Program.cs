using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Repositories.Interfaces;
using BlockedCountriesAPI.Services;
using BlockedCountriesAPI.Services.Interfaces;
using BlockedCountriesAPI.BackgroundServices;

namespace BlockedCountriesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

           
            // Register the service
            builder.Services.AddScoped<IGeolocationService, GeolocationService>();

            // Configure HttpClient specifically for GeolocationService
            builder.Services.AddHttpClient<GeolocationService>();

            //builder.Services.AddHttpClient<IGeolocationService, GeolocationService>(); // Alternative way 

            // Register Repositories
            builder.Services.AddSingleton<ICountryRepository, CountryRepository>();
            builder.Services.AddSingleton<IBlockedAttemptRepository, BlockedAttemptRepository>();

            // Register Services
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<IGeolocationService, GeolocationService>();
            builder.Services.AddScoped<IBlockedAttemptLogger, BlockedAttemptLogger>();

            // Register Background Services
            builder.Services.AddHostedService<TemporalBlockCleanupService>();

            // Add logging
            builder.Services.AddLogging();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
