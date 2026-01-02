# 🚗 Offer Service

The Offer Service is a core microservice in the automotive marketplace platform that manages vehicle offers and seller information. It provides comprehensive CRUD operations for vehicle listings and publishes events to maintain data consistency across the platform.

## 🏗️ Architecture

### Technology Stack
- **.NET 8**: Latest framework for high-performance APIs
- **FastEndpoints**: Lightweight alternative to MVC controllers
- **Entity Framework Core 8**: Modern ORM with SQL Server support
- **MassTransit**: Message bus for event-driven architecture
- **Swagger/OpenAPI**: Comprehensive API documentation

### Service Responsibilities
- ✅ Vehicle offer management (CRUD operations)
- ✅ Vehicle information storage and validation
- ✅ Seller data management
- ✅ Event publishing for offer lifecycle changes
- ✅ Data validation and business rules enforcement

## 🔧 Setup & Installation

### Prerequisites
- .NET 8 SDK
- SQL Server 2022+ (or Docker container)
- RabbitMQ (for event messaging)

### Local Development
```bash
# Clone and navigate
git clone <repository-url>
cd offer-service

# Restore dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the service
dotnet run --project AI.OfferService.Api
```

### Docker Deployment
```bash
# Build image
docker build -t automotive/offer-service .

# Run with dependencies
docker-compose up -d
```

## 🚀 API Endpoints

### Base URL
- **Development**: `http://localhost:5001`
- **Docker**: `http://localhost:5001`
- **Production**: Configure via environment variables

### Endpoints Overview

| Method | Endpoint | Description | Request | Response |
|--------|----------|-------------|---------|----------|
| `POST` | `/offers` | Create new offer | `CreateOfferRequest` | `OfferResponse` |
| `GET` | `/offers/{id}` | Get offer by ID | - | `OfferResponse` |
| `GET` | `/offers` | List offers with pagination | Query params | `OfferResponse[]` |
| `PUT` | `/offers/{id}` | Update existing offer | `UpdateOfferRequest` | `OfferResponse` |
| `DELETE` | `/offers/{id}` | Delete offer | - | `204 No Content` |
| `GET` | `/health` | Health check | - | `200 OK` |

### Sample Requests

#### Create Offer
```json
POST /offers
{
  "vin": "1HGCM82633A123456",
  "make": "Honda",
  "model": "Accord",
  "year": 2023,
  "trim": "EX-L",
  "sellerId": 12345,
  "buyerId": 67890,
  "carrierId": 11111,
  "offerAmount": 28500.00,
  "city": "Los Angeles",
  "state": "CA",
  "country": "USA"
}
```

#### Update Offer
```json
PUT /offers/1
{
  "offerAmount": 27500.00,
  "status": "NEGOTIATING"
}
```

#### List Offers
```
GET /offers?page=1&pageSize=20&sellerId=12345&status=OPEN
```

### Response Formats

#### Successful Offer Response
```json
{
  "id": 1,
  "vehicleId": 1,
  "sellerId": 12345,
  "buyerId": 67890,
  "carrierId": 11111,
  "offerAmount": 28500.00,
  "city": "Los Angeles",
  "state": "CA",
  "country": "USA",
  "status": "OPEN",
  "createdAt": "2026-01-02T10:30:00Z",
  "lastModifiedAt": "2026-01-02T10:30:00Z",
  "vehicle": {
    "id": 1,
    "make": "Honda",
    "model": "Accord",
    "year": 2023,
    "trim": "EX-L",
    "vin": "1HGCM82633A123456"
  }
}
```

## 🔄 Event Publishing

### Events Published
The service publishes events to RabbitMQ for consumption by other services:

#### OfferCreatedEvent
```json
{
  "offerId": 1,
  "vehicleId": 1,
  "sellerId": 12345,
  "buyerId": 67890,
  "carrierId": 11111,
  "offerAmount": 28500.00,
  "city": "Los Angeles",
  "state": "CA",
  "country": "USA",
  "status": "OPEN",
  "createdAt": "2026-01-02T10:30:00Z",
  "make": "Honda",
  "model": "Accord",
  "year": 2023,
  "trim": "EX-L",
  "vin": "1HGCM82633A123456"
}
```

#### OfferUpdatedEvent
```json
{
  "offerId": 1,
  "previousStatus": "OPEN",
  "newStatus": "NEGOTIATING",
  "offerAmount": 27500.00,
  "updatedAt": "2026-01-02T15:45:00Z"
}
```

### Event Configuration
Events are published using MassTransit with RabbitMQ as the transport:

```csharp
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});
```

## 📊 Database Schema

### Entities

#### Offer Table
| Column | Type | Description |
|--------|------|-------------|
| `Id` | `int` | Primary key, auto-increment |
| `VehicleId` | `int` | Foreign key to Vehicle table |
| `SellerId` | `int` | Seller identifier |
| `BuyerId` | `int?` | Buyer identifier (nullable) |
| `CarrierId` | `int?` | Carrier identifier (nullable) |
| `OfferAmount` | `decimal(18,2)` | Offer amount in USD |
| `City` | `nvarchar(100)` | City location |
| `State` | `nvarchar(50)` | State/province |
| `Country` | `nvarchar(50)` | Country |
| `Status` | `nvarchar(20)` | Offer status |
| `CreatedAt` | `datetime2` | Creation timestamp |
| `LastModifiedAt` | `datetime2` | Last update timestamp |

#### Vehicle Table
| Column | Type | Description |
|--------|------|-------------|
| `Id` | `int` | Primary key, auto-increment |
| `Make` | `nvarchar(50)` | Vehicle manufacturer |
| `Model` | `nvarchar(50)` | Vehicle model |
| `Year` | `int` | Model year |
| `Trim` | `nvarchar(50)` | Trim level (nullable) |
| `VIN` | `nvarchar(17)` | Vehicle Identification Number |
| `CreatedAt` | `datetime2` | Creation timestamp |
| `LastModifiedAt` | `datetime2` | Last update timestamp |

### Relationships
- `Offer.VehicleId` → `Vehicle.Id` (One-to-Many)
- VIN is unique across the Vehicle table
- Offers can exist without buyers/carriers initially

## ⚙️ Configuration

### Environment Variables
```bash
# Database Configuration
ConnectionStrings__DefaultConnection="Server=localhost;Database=AICodeChallenge2025;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"

# RabbitMQ Configuration  
RabbitMQ__Host="localhost"
RabbitMQ__Port=5672
RabbitMQ__Username="guest"
RabbitMQ__Password="guest"

# Application Settings
ASPNETCORE_ENVIRONMENT="Development"
ASPNETCORE_URLS="http://+:5001"
```

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "MassTransit": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AICodeChallenge2025;Integrated Security=true;TrustServerCertificate=True"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

## 🧪 Testing

### Unit Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests
```bash
# Start test dependencies
docker-compose -f docker-compose.test.yml up -d

# Run integration tests
dotnet test --filter Category=Integration
```

### Manual API Testing
```bash
# Health check
curl http://localhost:5001/health

# Create offer
curl -X POST http://localhost:5001/offers \
  -H "Content-Type: application/json" \
  -d '{"vin":"1HGCM82633A123456","make":"Honda","model":"Accord","year":2023,"sellerId":12345,"offerAmount":28500.00}'

# Get offers
curl http://localhost:5001/offers
```

## 🚨 Error Handling

### Standard HTTP Status Codes
- `200 OK`: Successful GET requests
- `201 Created`: Successful POST requests  
- `204 No Content`: Successful DELETE requests
- `400 Bad Request`: Validation errors
- `404 Not Found`: Resource not found
- `409 Conflict`: Duplicate VIN or business rule violations
- `500 Internal Server Error`: Unexpected server errors

### Error Response Format
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "vin": ["VIN must be exactly 17 characters"],
    "offerAmount": ["Offer amount must be greater than 0"]
  },
  "traceId": "80000001-0000-fd00-b63f-84710c7967bb"
}
```

## 📈 Performance Considerations

### Optimizations Implemented
- **Entity Framework Core**: Optimized queries with proper indexing
- **Pagination**: Efficient data retrieval for large datasets
- **Async Operations**: Non-blocking I/O operations
- **Connection Pooling**: Efficient database connection management
- **Event Publishing**: Asynchronous event publishing to avoid blocking

### Monitoring Metrics
- Request/response times
- Database query performance
- Event publishing latency
- Memory and CPU usage
- Error rates and types

## 🔒 Security Considerations

### Current Implementation
- Input validation using FluentValidation
- SQL injection protection via Entity Framework
- Proper error handling without information leakage

### Future Enhancements
- [ ] JWT authentication
- [ ] API rate limiting  
- [ ] Role-based authorization
- [ ] Input sanitization
- [ ] HTTPS enforcement
- [ ] CORS policy refinement

## 📚 Additional Resources

### Swagger Documentation
Access interactive API documentation at: `http://localhost:5001/swagger`

### Related Services
- [Purchase Service](../purchase-service/README.md) - Consumes offer events
- [Search Service](../search-service/README.md) - Indexes offer data
- [Transport Service](../transport-service/README.md) - Handles logistics

### External Dependencies
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [MassTransit](https://masstransit-project.com/)

## 🤝 Contributing

1. Follow the existing code style and patterns
2. Add unit tests for new features
3. Update API documentation
4. Ensure all events are properly published
5. Validate database migrations

---

**For support or questions, refer to the main [project documentation](../README.md) or open an issue.**