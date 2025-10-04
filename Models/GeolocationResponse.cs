using Newtonsoft.Json;

namespace BlockedCountriesAPI.Models
{
    public class GeolocationResponse
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        public string CountryCode => Location?.CountryCode2;
    }
    public class Location
    {
        [JsonProperty("continent_code")]
        public string ContinentCode { get; set; }

        [JsonProperty("continent_name")]
        public string ContinentName { get; set; }

        [JsonProperty("country_code2")]
        public string CountryCode2 { get; set; }

        [JsonProperty("country_code3")]
        public string CountryCode3 { get; set; }

        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        [JsonProperty("country_name_official")]
        public string CountryNameOfficial { get; set; }

        [JsonProperty("country_capital")]
        public string CountryCapital { get; set; }

        [JsonProperty("state_prov")]
        public string StateProvince { get; set; }

        [JsonProperty("state_code")]
        public string StateCode { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zipcode")]
        public string Zipcode { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("is_eu")]
        public bool IsEu { get; set; }
    }
}
