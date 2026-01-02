# Offer Service - Complete Implementation

This is a complete .NET 8 Web API implementation for an Offer Service built with FastEndpoints, Entity Framework Core, and RabbitMQ for event publishing.

## Project Structure

The solution consists of three main projects:

- **AI.OfferService.Api** - Web API layer with endpoints and configuration
- **AI.OfferService.Domain** - Domain layer with entities, repositories, and data context
- **AI.OfferService.Application** - Application layer with DTOs, services, and endpoints

## Features

- **CRUD Operations** for offers
- **Vehicle Management** with automatic creation based on VIN
- **Event Publishing** to RabbitMQ when offers are created
- **Entity Framework Core** with SQL Server
- **FastEndpoints** for clean API design
- **Swagger/OpenAPI** documentation
- **Docker** support with docker-compose

## API Endpoints

- `POST /api/offers` - Create a new offer
- `GET /api/offers` - List all offers (with optional sellerId filter)
- `GET /api/offers/{id}` - Get specific offer by ID
- `PUT /api/offers/{id}` - Update an existing offer
- `DELETE /api/offers/{id}` - Delete an offer

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (configured in appsettings.json)
- RabbitMQ (or use Docker Compose)

### Running Locally

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Run the application:**
   ```bash
   cd AI.OfferService.Api
   dotnet run
   ```

3. **Access Swagger UI:**
   Open `http://localhost:5001/swagger`

### Running with Docker

1. **Build and run with docker-compose:**
   ```bash
   docker-compose up --build
   ```

2. **Access the application:**
   - API: `http://localhost:5001`
   - Swagger: `http://localhost:5001/swagger`
   - RabbitMQ Management: `http://localhost:15672` (guest/guest)

## Database Configuration

The application uses Entity Framework Core with SQL Server. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server Connection String"
  }
}
```

## Event Publishing

When offers are created, an `OfferCreatedEvent` is published to RabbitMQ with routing key `offer.created` on the `automotive.exchange` topic exchange.

## Architecture

- **Clean Architecture** with separation of concerns
- **Repository Pattern** for data access
- **Event-Driven Architecture** with RabbitMQ
- **FastEndpoints** for minimal API approach
- **Domain-Driven Design** principles