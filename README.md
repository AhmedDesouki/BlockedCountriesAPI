# BlockedCountriesAPI

## Architecture
- The system implements a three-tier architecture.
 ### Presentation Layer (Controllers)
 - The API exposes three main controllers that handle HTTP requests:
 - CountriesController - manages country blocking operations
 - IpController - handles IP geolocation and blocking checks
 - LogsController - provides audit log access
 
 ### Service Layer (Business Logic)
 - The business logic is encapsulated in service interfaces and implementations:
 - ICountryService/CountryService - country blocking business logic
 - IGeolocationService/GeolocationService - IP geolocation functionality
 - IBlockedAttemptLogger/BlockedAttemptLogger - audit logging
 ### Repository Layer (Data Access)
 - Data persistence is handled through repository pattern:
 - ICountryRepository/CountryRepository - country block data storage
 - IBlockedAttemptRepository/BlockedAttemptRepository - audit log storage
 
  ### DTO Usage in Controllers
 - DTOs for input validation and response formatting across all three main controllers.

## Features
 - Country blocking/unblocking
 - IP geolocation and block checking
 - Temporal blocks with auto-expiry
 - Comprehensive logging

 ## Setup Instructions
### Prerequisites
 - .NET 8 SDK
 - IPGeolocation.io API key.

 ## API Documentation
 - Access Swagger UI at: `http://localhost:5224/swagger`.

 ## Key Endpoints
### Countries Management:

 - `POST /api/Countries/block` - Block country permanently
 - `POST /api/Countries/temporal-block` - Add temporal block 
 - `DELETE /api/Countries/block/{countryCode}` - Unblock country 
 - `GET /api/Countries/blocked` - Get blocked countries 
### IP Management:

 - `GET /api/Ip/lookup` - IP geolocation lookup 
 - `GET /api/Ip/check-block` - Check if IP is blocked 
### Audit Logging:

 - `GET /api/Logs/blocked-attempts` - Get blocked attempts logs 



 ![Profile view counter on GitHub](https://komarev.com/ghpvc/?username=AhmedDesouki)

