# Offer Service - Complete Implementation

A comprehensive .NET 8 Web API implementation for managing vehicle offers in an automotive marketplace. This microservice provides complete CRUD operations for offers, automatic vehicle management, and event-driven architecture using RabbitMQ for inter-service communication.

## Table of Contents

- [Project Overview](#project-overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Running Locally](#running-locally)
  - [Running with Docker Compose](#running-with-docker-compose)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Event Publishing](#event-publishing)
- [Architecture](#architecture)
- [Development](#development)
- [Troubleshooting](#troubleshooting)

## Project Overview

The Offer Service is designed to handle the complete lifecycle of vehicle offers in an automotive marketplace. It allows sellers to create offers for vehicles, buyers to view and interact with offers, and carriers to track transportation details. The service automatically manages vehicle information based on VIN (Vehicle Identification Number) and publishes events to notify other services when offers are created.

## Technology Stack

- **.NET 8** - Latest LTS version of the .NET framework
- **FastEndpoints 5.30.0** - High-performance alternative to ASP.NET Core MVC/Minimal APIs
- **Entity Framework Core 8.0** - ORM for database operations
- **SQL Server** - Primary data store
- **RabbitMQ 3 (with Management Plugin)** - Message broker for event-driven architecture
- **Swagger/OpenAPI** - API documentation and testing interface
- **Docker & Docker Compose** - Containerization and orchestration

## Project Structure

The solution follows Clean Architecture principles with clear separation of concerns:

```
OfferService/
├── AI.OfferService.Api/              # API Layer
│   ├── Program.cs                    # Application entry point and DI configuration
│   ├── appsettings.json             # Configuration settings
│   └── AI.OfferService.Api.csproj   # Project dependencies
│
├── AI.OfferService.Application/      # Application Layer
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── CreateOfferRequest.cs    # Request DTO for creating offers
│   │   ├── UpdateOfferRequest.cs    # Request DTO for updating offers
│   │   └── OfferResponse.cs         # Response DTO for offer data
│   ├── Endpoints/                    # FastEndpoints implementations
│   │   ├── CreateOfferEndpoint.cs   # POST /api/offers
│   │   ├── GetOfferEndpoint.cs      # GET /api/offers/{id}
│   │   ├── ListOffersEndpoint.cs    # GET /api/offers
│   │   ├── UpdateOfferEndpoint.cs   # PUT /api/offers/{id}
│   │   └── DeleteOfferEndpoint.cs   # DELETE /api/offers/{id}
│   ├── Services/                     # Business logic services
│   │   ├── IEventPublisher.cs       # Event publisher interface
│   │   ├── NoOpEventPublisher.cs    # Development/testing implementation
│   │   └── RabbitMQEventPublisher.cs # Production RabbitMQ implementation
│   └── Events/                       # Event definitions
│
├── AI.OfferService.Domain/           # Domain Layer
│   ├── Entities/                     # Domain entities
│   │   ├── Offer.cs                 # Core offer entity
│   │   └── Vehicle.cs               # Vehicle entity
│   ├── Repositories/                 # Repository interfaces and implementations
│   │   ├── IOfferRepository.cs      # Offer repository interface
│   │   ├── OfferRepository.cs       # Offer repository implementation
│   │   ├── IVehicleRepository.cs    # Vehicle repository interface
│   │   └── VehicleRepository.cs     # Vehicle repository implementation
│   └── Data/                         # Data access layer
│       └── OfferDbContext.cs        # EF Core DbContext
│
├── Dockerfile                        # Docker image configuration
├── docker-compose.yml                # Multi-container setup (API + RabbitMQ)
└── OfferService.sln                 # Solution file
```

## Features

### Core Functionality
- **Complete CRUD Operations** - Create, Read, Update, and Delete offers
- **Automatic Vehicle Management** - Vehicles are automatically created or retrieved based on VIN
- **Offer Status Tracking** - Support for OPEN, SOLD, and CANCELLED statuses
- **Filtering Capabilities** - List offers with optional filtering by seller ID
- **Location Tracking** - Track offer location (City, State, Country)

### Technical Features
- **Event-Driven Architecture** - Publishes events to RabbitMQ when offers are created
- **Environment-Aware Configuration** - Uses NoOp publisher in development, RabbitMQ in production
- **Snake Case JSON** - API uses snake_case for JSON properties
- **Clean Architecture** - Separation of concerns with distinct layers
- **Repository Pattern** - Abstraction over data access logic
- **Docker Support** - Full containerization with Docker Compose
- **API Documentation** - Interactive Swagger UI for testing and documentation
- **Entity Relationships** - Properly configured one-to-many relationship between Vehicle and Offers

## Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 8 SDK** (version 8.0 or higher) - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (2019 or later) or **SQL Server Express** - [Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
  - Alternatively, use the provided remote SQL Server instance
- **Docker Desktop** (optional, for containerized deployment) - [Download here](https://www.docker.com/products/docker-desktop/)
- **RabbitMQ** (optional, for local development without Docker) - [Download here](https://www.rabbitmq.com/download.html)
  - Or use Docker Compose which includes RabbitMQ

## Getting Started

### Running Locally

#### 1. Clone the Repository
```bash
git clone https://github.com/maitridave/offer-service.git
cd offer-service
```

#### 2. Configure Database Connection
Update the connection string in `AI.OfferService.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OfferServiceDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

#### 3. Set Up the Database
The application will automatically create the database schema on first run using Entity Framework Core. If you need to create migrations manually:

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Navigate to the API project
cd AI.OfferService.Api

# Create a new migration (if schema changes are needed)
dotnet ef migrations add InitialCreate --project ../AI.OfferService.Domain

# Apply migrations to the database
dotnet ef database update --project ../AI.OfferService.Domain
```

#### 4. Restore Dependencies
```bash
# From the solution root
dotnet restore
```

#### 5. Build the Solution
```bash
dotnet build
```

#### 6. Run the Application
```bash
cd AI.OfferService.Api
dotnet run
```

The application will start on `http://localhost:5001` (or the port specified in your launch settings).

#### 7. Access Swagger UI
Open your browser and navigate to:
```
http://localhost:5001/swagger
```

### Running with Docker Compose

Docker Compose is the recommended way to run the application as it includes all dependencies (RabbitMQ and the API).

#### 1. Start All Services
```bash
# From the solution root
docker-compose up --build
```

This command will:
- Build the .NET application Docker image
- Pull the RabbitMQ image with management plugin
- Start both containers
- Create a shared network for communication

#### 2. Access the Services
- **API**: http://localhost:5001
- **Swagger UI**: http://localhost:5001/swagger
- **RabbitMQ Management**: http://localhost:15672
  - Username: `guest`
  - Password: `guest`

#### 3. Stop the Services
```bash
# Stop and remove containers
docker-compose down

# Stop, remove containers, and clean up volumes
docker-compose down -v
```

#### 4. View Logs
```bash
# View all logs
docker-compose logs

# View logs for specific service
docker-compose logs offer-service
docker-compose logs rabbitmq

# Follow logs in real-time
docker-compose logs -f
```

## Configuration

### Application Settings (`appsettings.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OfferServiceDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Environment Variables (Docker Compose)

When running with Docker Compose, you can override settings using environment variables:

- `ASPNETCORE_ENVIRONMENT` - Set to "Development" or "Production"
- `ConnectionStrings__DefaultConnection` - SQL Server connection string
- `RabbitMQ__Host` - RabbitMQ host (default: "rabbitmq" in Docker)
- `RabbitMQ__Port` - RabbitMQ port (default: 5672)
- `RabbitMQ__Username` - RabbitMQ username
- `RabbitMQ__Password` - RabbitMQ password

### Event Publisher Configuration

The application automatically selects the event publisher based on the environment:

- **Development** - Uses `NoOpEventPublisher` (no-operation, events are logged but not published)
- **Production** - Uses `RabbitMQEventPublisher` (publishes to RabbitMQ)

## API Documentation

### Endpoints

#### 1. Create Offer
**POST** `/api/offers`

Creates a new offer and automatically manages the associated vehicle.

**Request Body:**
```json
{
  "seller_id": 101,
  "buyer_id": 201,
  "carrier_id": 301,
  "offer_amount": 25000.00,
  "city": "San Francisco",
  "state": "CA",
  "country": "USA",
  "make": "Tesla",
  "model": "Model 3",
  "year": 2023,
  "trim": "Long Range",
  "vin": "5YJ3E1EA1KF123456"
}
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "vehicle_id": 1,
  "seller_id": 101,
  "buyer_id": 201,
  "carrier_id": 301,
  "offer_amount": 25000.00,
  "city": "San Francisco",
  "state": "CA",
  "country": "USA",
  "status": "OPEN",
  "created_at": "2026-01-02T18:00:00Z",
  "last_modified_at": "2026-01-02T18:00:00Z",
  "vehicle": {
    "id": 1,
    "make": "Tesla",
    "model": "Model 3",
    "year": 2023,
    "trim": "Long Range",
    "vin": "5YJ3E1EA1KF123456"
  }
}
```

#### 2. Get Offer by ID
**GET** `/api/offers/{id}`

Retrieves a specific offer by its ID.

**Response:** `200 OK`
```json
{
  "id": 1,
  "vehicle_id": 1,
  "seller_id": 101,
  "buyer_id": 201,
  "carrier_id": 301,
  "offer_amount": 25000.00,
  "city": "San Francisco",
  "state": "CA",
  "country": "USA",
  "status": "OPEN",
  "created_at": "2026-01-02T18:00:00Z",
  "last_modified_at": "2026-01-02T18:00:00Z",
  "vehicle": {
    "id": 1,
    "make": "Tesla",
    "model": "Model 3",
    "year": 2023,
    "trim": "Long Range",
    "vin": "5YJ3E1EA1KF123456"
  }
}
```

#### 3. List All Offers
**GET** `/api/offers?sellerId={sellerId}`

Lists all offers with optional filtering by seller ID.

**Query Parameters:**
- `sellerId` (optional) - Filter offers by seller ID

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "vehicle_id": 1,
    "seller_id": 101,
    "buyer_id": 201,
    "carrier_id": 301,
    "offer_amount": 25000.00,
    "city": "San Francisco",
    "state": "CA",
    "country": "USA",
    "status": "OPEN",
    "created_at": "2026-01-02T18:00:00Z",
    "last_modified_at": "2026-01-02T18:00:00Z",
    "vehicle": {
      "id": 1,
      "make": "Tesla",
      "model": "Model 3",
      "year": 2023,
      "trim": "Long Range",
      "vin": "5YJ3E1EA1KF123456"
    }
  }
]
```

#### 4. Update Offer
**PUT** `/api/offers/{id}`

Updates an existing offer.

**Request Body:**
```json
{
  "buyer_id": 202,
  "carrier_id": 302,
  "offer_amount": 26000.00,
  "city": "Los Angeles",
  "state": "CA",
  "country": "USA",
  "status": "SOLD"
}
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "vehicle_id": 1,
  "seller_id": 101,
  "buyer_id": 202,
  "carrier_id": 302,
  "offer_amount": 26000.00,
  "city": "Los Angeles",
  "state": "CA",
  "country": "USA",
  "status": "SOLD",
  "created_at": "2026-01-02T18:00:00Z",
  "last_modified_at": "2026-01-02T18:30:00Z",
  "vehicle": {
    "id": 1,
    "make": "Tesla",
    "model": "Model 3",
    "year": 2023,
    "trim": "Long Range",
    "vin": "5YJ3E1EA1KF123456"
  }
}
```

#### 5. Delete Offer
**DELETE** `/api/offers/{id}`

Deletes an offer by its ID.

**Response:** `204 No Content`

## Database Schema

### Offers Table
| Column | Type | Description |
|--------|------|-------------|
| Id | bigint | Primary key, auto-increment |
| VehicleId | bigint | Foreign key to Vehicles table |
| SellerId | bigint | ID of the seller |
| BuyerId | bigint | ID of the buyer |
| CarrierId | bigint | ID of the carrier |
| OfferAmount | decimal(18,2) | Offer price amount |
| City | nvarchar(max) | Offer location city |
| State | nvarchar(max) | Offer location state |
| Country | nvarchar(max) | Offer location country |
| Status | nvarchar(max) | Offer status (OPEN, SOLD, CANCELLED) |
| CreatedAt | datetime2 | Record creation timestamp |
| LastModifiedAt | datetime2 | Last update timestamp |

### Vehicles Table
| Column | Type | Description |
|--------|------|-------------|
| Id | bigint | Primary key, auto-increment |
| Make | nvarchar(max) | Vehicle manufacturer |
| Model | nvarchar(max) | Vehicle model |
| Year | int | Manufacturing year |
| Trim | nvarchar(max) | Vehicle trim level |
| VIN | nvarchar(max) | Vehicle Identification Number (unique) |
| CreatedAt | datetime2 | Record creation timestamp |
| LastModifiedAt | datetime2 | Last update timestamp |

**Relationship:** One Vehicle can have many Offers (One-to-Many)

## Event Publishing

### OfferCreatedEvent

When a new offer is created, an `OfferCreatedEvent` is published to RabbitMQ with the following structure:

**Exchange:** `automotive.exchange` (Topic Exchange)
**Routing Key:** `offer.created`

**Event Payload:**
```json
{
  "offer_id": 1,
  "vehicle_id": 1,
  "seller_id": 101,
  "buyer_id": 201,
  "carrier_id": 301,
  "offer_amount": 25000.00,
  "city": "San Francisco",
  "state": "CA",
  "country": "USA",
  "status": "OPEN",
  "created_at": "2026-01-02T18:00:00Z",
  "vin": "5YJ3E1EA1KF123456"
}
```

Other microservices can subscribe to this event to react to new offers (e.g., notification service, analytics service, etc.).

## Architecture

### Design Patterns and Principles

1. **Clean Architecture**
   - Clear separation between API, Application, and Domain layers
   - Dependencies point inward (Domain has no dependencies)
   - Business logic isolated from infrastructure concerns

2. **Repository Pattern**
   - Abstraction over data access logic
   - Easy to test and mock
   - Encapsulates EF Core operations

3. **Event-Driven Architecture**
   - Loose coupling between services
   - Asynchronous communication via RabbitMQ
   - Scalable and resilient design

4. **FastEndpoints Pattern**
   - Each endpoint is a separate class
   - Vertical slice architecture
   - Better organization and testability than traditional controllers

5. **Domain-Driven Design (DDD)**
   - Rich domain entities (Offer, Vehicle)
   - Repositories for aggregate roots
   - Clear ubiquitous language

### Application Flow

1. **Request Received** → FastEndpoint receives HTTP request
2. **Validation** → Request DTO is validated
3. **Repository Access** → Repository fetches/persists data via EF Core
4. **Business Logic** → Application service processes the business rules
5. **Event Publishing** → Event publisher sends message to RabbitMQ (if applicable)
6. **Response** → DTO is serialized and returned to client

## Development

### Building the Project
```bash
# Build all projects
dotnet build

# Build in Release mode
dotnet build -c Release

# Build specific project
dotnet build AI.OfferService.Api/AI.OfferService.Api.csproj
```

### Running Tests
```bash
# Run all tests (if test projects exist)
dotnet test

# Run with detailed output
dotnet test --verbosity detailed
```

### Code Style
- Follow standard C# naming conventions
- Use `async/await` for asynchronous operations
- Enable nullable reference types
- Use implicit usings (enabled by default in .NET 8)

### Adding New Endpoints
1. Create a new endpoint class in `AI.OfferService.Application/Endpoints/`
2. Inherit from `Endpoint<TRequest, TResponse>` or appropriate FastEndpoints base class
3. Override `Configure()` to set up routing and verb
4. Override `HandleAsync()` to implement logic
5. Register dependencies in `Program.cs` if needed

### Database Migrations
```bash
# Add a new migration
dotnet ef migrations add MigrationName --project AI.OfferService.Domain --startup-project AI.OfferService.Api

# Update database
dotnet ef database update --project AI.OfferService.Domain --startup-project AI.OfferService.Api

# Remove last migration (if not applied)
dotnet ef migrations remove --project AI.OfferService.Domain --startup-project AI.OfferService.Api
```

## Troubleshooting

### Issue: Database Connection Fails
**Solution:**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure firewall allows connections on port 1433
- Verify SQL Server authentication mode (Mixed Mode for SQL auth)
- Check `TrustServerCertificate=True` is set if using self-signed certificates

### Issue: RabbitMQ Connection Fails
**Solution:**
- Ensure RabbitMQ is running (check with `docker ps` if using Docker)
- Verify RabbitMQ configuration in `appsettings.json`
- Check RabbitMQ Management UI at http://localhost:15672
- In development, the app uses NoOpEventPublisher, so RabbitMQ is optional

### Issue: Port Already in Use
**Solution:**
```bash
# Check what's using the port (Linux/Mac)
lsof -i :5001

# Check what's using the port (Windows)
netstat -ano | findstr :5001

# Kill the process or change the port in launchSettings.json
```

### Issue: Docker Build Fails
**Solution:**
- Ensure Docker Desktop is running
- Check Docker daemon status
- Clear Docker cache: `docker system prune -a`
- Verify Dockerfile syntax
- Check for sufficient disk space

### Issue: Swagger UI Not Loading
**Solution:**
- Verify the app is running
- Check the correct URL: `http://localhost:5001/swagger`
- Ensure FastEndpoints.Swagger package is installed
- Check `Program.cs` has `app.UseSwaggerGen()` configured

### Issue: EF Core Migrations Not Working
**Solution:**
- Install EF Core tools: `dotnet tool install --global dotnet-ef`
- Ensure `Microsoft.EntityFrameworkCore.Design` package is installed
- Verify connection string is correct
- Run migrations from the correct directory (API project)

---

**For additional help or to report issues, please open an issue on GitHub.**

**License:** MIT (or specify your license)

**Contributors:** Maitri Dave and contributors