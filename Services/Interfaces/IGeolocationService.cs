using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Services.Interfaces
{
    public interface IGeolocationService
    {
        Task<string> GetCallerIpAsync();
        Task<GeolocationResponse> GetGeolocationAsync(string ipAddress);
        bool IsValidIpAddress(string ipAddress);
    }
}
