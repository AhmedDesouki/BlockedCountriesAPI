using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;
using BlockedCountriesAPI.DTOs;

namespace BlockedCountriesAPI.Services
{
    public class GeolocationService : IGeolocationService
    {
        
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeolocationService> _logger;

        public GeolocationService(HttpClient httpClient, IConfiguration configuration, ILogger<GeolocationService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeolocationApi:ApiKey"];
            _logger = logger;

            _httpClient.BaseAddress = new Uri("https://api.ipgeolocation.io/v2/");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BlockedCountriesAPI/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        public async Task<string> GetCallerIpAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("getip");

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"GetIP API returned {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var ipResponse = JsonConvert.DeserializeObject<IpResponse>(content);

                return ipResponse?.Ip;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting caller IP");
                throw new Exception($"Failed to get caller IP: {ex.Message}");
            }
        }

        public async Task<GeolocationResponse> GetGeolocationAsync(string ipAddress)
        {
            try
            {
                var url = $"ipgeo?apiKey={_apiKey}&ip={ipAddress}&fields=continent_code,continent_name,country_code2,country_code3,country_name,country_name_official,country_capital,state_prov,state_code,district,city,zipcode,latitude,longitude,is_eu";

                _logger.LogInformation("Calling geolocation API for IP: {IpAddress}", ipAddress);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Geolocation API returned {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Geolocation API returned {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GeolocationResponse>(content);

                if (result?.Location == null)
                {
                    throw new Exception("Invalid geolocation response");
                }

                _logger.LogInformation("Geolocation found: {CountryName} ({CountryCode})",
                    result.Location.CountryName, result.Location.CountryCode2);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling geolocation service for IP: {IpAddress}", ipAddress);
                throw new Exception($"Geolocation service error: {ex.Message}");
            }
        }

        public bool IsValidIpAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            return IPAddress.TryParse(ipAddress, out _);
        }
    }
}

