namespace BlockedCountriesAPI.Models
{
    public class CountryBlock
    {
        public string CountryCode { get; set; }
        public DateTime BlockedAt { get; set; }
        public bool IsTemporary { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
