using Newtonsoft.Json;

namespace BlockedCountriesAPI.DTOs
{
    public class IpResponse
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }
    }
}
