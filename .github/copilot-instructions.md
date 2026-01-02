---
description: 'Comprehensive development guidelines for Peddle Domain-Driven Microservices using MassTransit and RabbitMQ'
applyTo: '**/*.cs, **/*.json, **/*.md'
---

# Peddle Domain-Driven Microservice Development Guidelines

## Project Overview
This document provides standardized development guidelines for **Peddle microservices** built with **ASP.NET Core Worker Service**, leveraging **MassTransit** with **RabbitMQ** and **Domain-Driven Design (DDD)** principles for business data processing and integration.

---

## Core Architecture Principles

### Project Structure (Domain-Driven Design)
- **`src/`** – Main Worker Service project using domain-driven architecture
- **`src/Domain/`** – Domain layer organized by bounded contexts and business capabilities
  - **`{Feature}/`** – Feature-specific folders containing domain logic
    - **`{CommandName}CommandConsumer.cs`** – Command consumers implementing `IConsumer<TCommand>`
    - **`{EventName}EventConsumer.cs`** – Event consumers implementing `IConsumer<TEvent>` 
    - **Domain Services** – Business logic implementations
    - **Entities** – Domain entities and value objects
    - **Repositories** – Data access interfaces and implementations
  - **Cross-cutting Domain Services** – Shared domain services
    - **`I{ServiceName}.cs`** – Service interfaces
    - **`{ServiceName}.cs`** – Service implementations
- **`src/Shared/`** – Shared infrastructure and cross-cutting concerns
  - **`DependencyInjectionExtension.cs`** – Service registration and message broker configuration
  - **`AppConstants.cs`** – Application constants
  - **Configuration classes** – External service configurations
- **`src/Properties/`** – Launch settings and configurations
- **`tests/UnitTests/`** – Comprehensive test suite organized by domain
  - **`Domain/`** – Domain logic and service tests
  - **`Helper/`** – Helper and utility tests  

---

## Technology Stack
- **Framework**: ASP.NET Core 8.0+ Worker Service with MassTransit
- **Message Broker**: RabbitMQ with **Peddle.Foundation.Messagebroker** library integration
- **Architecture**: Domain-Driven Design (DDD) with message-driven processing (Commands and Events)
- **Message Types**: Support for both Commands (internal operations) and Events (external integrations)
- **Logging & Observability**: Serilog with structured logging
- **Configuration**: AWS Systems Manager Parameter Store
- **Object Mapping**: AutoMapper for data transformations
- **Testing**: xUnit + MOQ for unit testing
- **Health Checks**: TinyHealthCheck for service monitoring
- **Database**: Entity Framework Core with PostgreSQL or SQL Server

---

## Development Guidelines

### Domain-Driven Message Processing Implementation
- Use **Domain-focused Message Consumers** that implement `IConsumer<TCommand>` or `IConsumer<TEvent>` interface
- Organize consumers by **business capability** rather than technical layers
- **Commands**: Handle internal business operations and processing workflows
- **Events**: Handle external system integrations and cross-service communication
- Each consumer handles a specific business capability or integration need
- Domain services encapsulate complex business logic and external integrations
- Repository pattern for data access with domain-specific interfaces

#### Example Domain Structure
```
src/Domain/
├── ProcessingFeature/
│   ├── ProcessDataCommandConsumer.cs          # Internal business operation
│   ├── DataProcessedEventConsumer.cs          # External event handling
│   ├── ProcessingService.cs
│   ├── IProcessingService.cs
│   ├── ProcessingEntity.cs
│   ├── ProcessingRepository.cs
│   ├── IProcessingRepository.cs
│   └── ProcessingFeatureMappingProfile.cs     # AutoMapper profile for feature
├── IntegrationFeature/
│   ├── SyncDataCommandConsumer.cs             # Internal sync operation  
│   ├── ExternalSystemEventConsumer.cs         # External system event
│   ├── IntegrationService.cs
│   ├── IIntegrationService.cs
│   ├── IntegrationEntity.cs
│   ├── IntegrationRepository.cs
│   ├── IIntegrationRepository.cs
│   └── IntegrationFeatureMappingProfile.cs    # AutoMapper profile for feature
├── DatabaseIntegration/
│   ├── FeatureEntity.cs
│   ├── FeatureRepository.cs
│   ├── IFeatureRepository.cs
│   └── FeatureDatabaseContext.cs
├── SharedService.cs
├── ISharedService.cs
├── SharedEntity.cs
├── SharedRepository.cs
├── ISharedRepository.cs

```

### MassTransit Configuration with Peddle.Foundation.Messagebroker

#### Domain-Driven Configuration Setup
Configure MassTransit in `DependencyInjectionExtension.cs` organizing consumers by domain capability:

```csharp
private static IServiceCollection RegisterMassTransitMessageBroker(
    this IServiceCollection services, IConfiguration configuration)
{
    services.RegisterBroker(options =>
    {
        // Connection configuration
        options.HostName = configuration.GetValue<string>("Configs:Rabbitmq:HostName");
        options.PortNumber = configuration.GetValue<int>("Configs:Rabbitmq:PortNumber");
        options.Username = configuration.GetValue<string>("Secrets:Rabbitmq:UserName");
        options.Password = Utility.GetSecureString(configuration.GetValue<string>("Secrets:Rabbitmq:UserPassword"));
        
        // Identity provider configuration for authentication
        options.IdentityProviderServiceHost = configuration.GetValue<string>("IDENTITY_PROVIDER_SERVICE_HOST");
        options.GenericMessageBrokerClientId = configuration.GetValue<string>("Secrets:ApiClient:GenericMessageBrokerClientId");
        options.GenericMessageBrokerClientSecret = configuration.GetValue<string>("Secrets:ApiClient:GenericMessageBrokerClientSecret");
        
        // Subscriber configuration organized by domain capabilities
        options.SubscriberTopologyOptions = topologyOptions =>
        {
            topologyOptions.ApplicationName = Subscriber.YourMicroserviceName.ToString();
            
            topologyOptions.SubscriberOptions = new List<Action<SubscriberOptions>>
            {
                // Data Processing Domain - Commands (Internal Operations)
                processDataCommandConsumer =>
                {
                    processDataCommandConsumer.Name = nameof(ProcessDataCommandConsumer);
                    processDataCommandConsumer.Consumer = typeof(ProcessDataCommandConsumer);
                    processDataCommandConsumer.MessagesToSubscribe = [ typeof(ProcessDataCommand) ];
                },
                
                validateDataCommandConsumer =>
                {
                    validateDataCommandConsumer.Name = nameof(ValidateDataCommandConsumer);
                    validateDataCommandConsumer.Consumer = typeof(ValidateDataCommandConsumer);
                    validateDataCommandConsumer.MessagesToSubscribe = [ typeof(ValidateDataCommand) ];
                },
                
                // Integration Domain - Events (External System Integration)
                externalSystemEventConsumer =>
                {
                    externalSystemEventConsumer.Name = nameof(ExternalSystemEventConsumer);
                    externalSystemEventConsumer.Consumer = typeof(ExternalSystemEventConsumer);
                    externalSystemEventConsumer.MessagesToSubscribe = [ typeof(ExternalDataUpdatedEvent) ];
                },
                
                thirdPartyEventConsumer =>
                {
                    thirdPartyEventConsumer.Name = nameof(ThirdPartyEventConsumer);
                    thirdPartyEventConsumer.Consumer = typeof(ThirdPartyEventConsumer);
                    thirdPartyEventConsumer.MessagesToSubscribe = [ typeof(ThirdPartySystemEvent) ];
                },
                
            };
        };
    }, true);
    
    return services;
}
```

#### Simplified Message Sending Pattern

The Peddle.Foundation.Messagebroker library simplifies command sending by automatically resolving endpoints:

```csharp
// Example command sending in business workflow
private async Task SendNextCommand(string requestId, object processedData)
{
    await _sender.Send(new FinalizeProcessCommand(DateTime.Now) 
    { 
        RequestId = requestId,
        Data = processedData 
    });
}

// Example conditional workflow branching
private async Task SendNextCommand(string requestId, ProcessingContext context)
{
    if (context.RequiresExternalIntegration)
    {
        await _sender.Send(new SyncWithExternalSystemCommand(DateTime.Now) 
        { 
            RequestId = requestId,
            ExternalSystemId = context.ExternalSystemId 
        });
    }
    else
    {
        await _sender.Send(new CompleteProcessingCommand(DateTime.Now) 
        { 
            RequestId = requestId 
        });
    }
}
```

**Key Benefits:**
- No need to manually specify endpoints
- Endpoint resolution based on application name in CommandBase
- Simplified configuration and reduced boilerplate code

### Domain Service Integration Patterns

#### External Service Integration Pattern
- Use wrapper pattern for external service operations with retry logic and error handling (recommend using Polly for robust retry strategies)
- Implement domain-specific service abstractions for external systems
- Handle external service-specific error scenarios with proper retry strategies (Polly is recommended for transient fault handling)
- Encapsulate external system communication complexity behind domain interfaces

#### API Integration Pattern
- Use domain services for external API operations with proper error handling
- Implement mapping between domain entities and external system DTOs
- Handle rate limiting and API quotas in service implementations
- Use configuration objects for service-specific settings


#### Database Integration Pattern
- Use Entity Framework Core with PostgreSQL or SQL Server as the database provider.
- All entities should inherit from `BaseEntity` from `Peddle.Foundation.Data.Repositories`.
- Repository interfaces should inherit from `IRepository<TEntity>` where TEntity is your domain entity.
- Repository implementations should inherit from `Repository<TEntity>` and implement your custom interface.
- Use the constructor pattern: `Repository<TEntity>(DbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)`.
- The following async/await data access methods are already implemented in the parent class `Repository` in `Peddle.Foundation.Data.Repositories`:
    - `GetAsync(Expression<Func<TEntity, bool>> predicate)` - Single entity retrieval with predicate
    - `GetListAsync(Expression<Func<TEntity, bool>> predicate)` - Multiple entities with predicate
    - `GetListAsync()` - All entities
    - `Queryable(Expression<Func<TEntity, bool>> predicate)` - For complex LINQ queries
    - `Add(TEntity entity)` - Add single entity
    - `AddRange(IEnumerable<TEntity> entities)` - Add multiple entities
    - `Update(TEntity entity)` - Update single entity
    - `Remove(TEntity entity)` - Remove single entity
    - `SaveChangesAsync()` - Save without audit trail
    - `SaveChangesAsync(string userId)` - Save with audit trail logging
Always reuse these existing methods in inherited repositories wherever possible, instead of creating new methods.
- For entity configuration, always ignore `CreatedDateTime` and `LastModifiedDateTime` in favor of `CreatedAt`/`LastModifiedAt`.

**Entity Configuration Example:**
```csharp
public class EntityConfiguration : IEntityTypeConfiguration<EntityName>
{
    public void Configure(EntityTypeBuilder<EntityName> builder)
    {
        builder.Ignore(x => x.CreatedDateTime);
        builder.Ignore(x => x.LastModifiedDateTime);
        // Only map CreatedAt and LastModifiedAt if they exist in database
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
        builder.Property(e => e.LastModifiedAt).HasColumnName("LastModifiedAt");
    }
}
```

**Repository Example:**
```csharp
public interface IDataRepository : IRepository<DataEntity>
{
    Task<IEnumerable<DataEntity>> GetDataByTypeAsync(string type);
    Task<IReadOnlyList<int>> GetUniqueYearsAsync();
}

public class DataRepository(DatabaseContext dbContext, ILoggerFactory loggerFactory)
    : Repository<DataEntity>(dbContext, loggerFactory), IDataRepository
{
    public async Task<IEnumerable<DataEntity>> GetDataByTypeAsync(string type)
    {
        return await GetListAsync(d => d.Type.Equals(type));
    }
    public async Task<IReadOnlyList<int>> GetUniqueYearsAsync()
    {
        return await Queryable(d => true)
            .Select(d => d.Year)
            .Distinct()
            .OrderByDescending(year => year)
            .ToListAsync();
    }
}
```


### Caching Strategy
- Implement Redis caching for frequently accessed or expensive-to-compute data.
- Use `ICacheService<T>` with region-based cache management and meaningful cache key conventions.
- Integrate caching in domain services and consumers where appropriate.
- Configure cache expiration and invalidation policies as needed.

**Caching Example:**
```csharp
public class ExampleDomainService : IExampleDomainService
{
    private readonly ICacheService<DataModel> _cacheService;
    private readonly ILogger<ExampleDomainService> _logger;

    public ExampleDomainService(ICacheService<DataModel> cacheService, ILogger<ExampleDomainService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<DataModel> GetDataAsync(string id)
    {
        var cacheKey = $"DataModel_{id}";
        _logger.LogInformation("Fetching data for id {Id} with cache key {CacheKey}", id, cacheKey);
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            // ... fetch data from repository or external source ...
            return new DataModel();
        });
    }
}
```

### Logging and Observability
- Always use `ILogger<T>` for logging in consumers, domain services, and repositories.
- Log key actions, errors, and message details at appropriate log levels.
- Use Serilog for structured logging and follow best practices as outlined in [Serilog Dos and Don'ts](https://esg.dev/posts/serilog-dos-and-donts/).
- Include correlation IDs and relevant business context in logs for distributed tracing.

**Logging Example in Consumer:**
```csharp
public class ProcessDataCommandConsumer : IConsumer<ProcessDataCommand>
{
    private readonly ILogger<ProcessDataCommandConsumer> _logger;
    public ProcessDataCommandConsumer(ILogger<ProcessDataCommandConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<ProcessDataCommand> context)
    {
        _logger.LogInformation("Processing command for RequestId {RequestId}", context.Message.RequestId);
        try
        {
            // ... business logic ...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing command for RequestId {RequestId}", context.Message.RequestId);
            throw;
        }
    }
}
```

**Serilog Configuration Example:**
```csharp
builder.Host.UseSerilog((context, _, loggerConfiguration) => 
    loggerConfiguration.ReadFrom.Configuration(context.Configuration).EnableSensitiveDataMasking());
```

## Exception Handling Patterns

### BusinessException Usage
- **Use Standard Exceptions**: For this domain-driven architecture, use appropriate .NET exceptions (`InvalidOperationException`, `ArgumentException`, etc.) for domain validation
- **Domain-Specific Exceptions**: Create domain-specific exceptions when needed for business rule violations
- **Error Context**: Include relevant domain context in exception messages for better debugging
- **Pipeline Error Handling**: Implement comprehensive error handling in each pipeline stage to prevent cascade failures

```csharp
// Domain validation example
public async Task Consume(ConsumeContext<ProcessDataCommand> context)
{
    try
    {
        _logger.LogInformation("ProcessDataCommand Started for RequestId {@RequestId}", context.Message.RequestId);
        
        // Domain logic with validation
        if (string.IsNullOrEmpty(context.Message.RequestId))
        {
            throw new ArgumentException("RequestId cannot be null or empty", nameof(context.Message.RequestId));
        }
        
        var processedData = await _processingService.ProcessAsync(
            context.Message.Data, context.Message.Options);
        
        if (processedData == null)
        {
            throw new InvalidOperationException($"Processing failed for RequestId {context.Message.RequestId}");
        }
        
        // Continue workflow or complete processing
        if (processedData.RequiresFurtherProcessing)
        {
            await SendNextCommand(context.Message.RequestId, processedData);
        }
        
        _logger.LogInformation("ProcessDataCommand Completed for RequestId {@RequestId}", context.Message.RequestId);
    }
    catch (Exception e)
    {
        _logger.LogError(e, "ProcessDataCommand failed for RequestId {@RequestId}: {@error}", 
            context.Message.RequestId, e.Message);
        throw; // Re-throw to trigger MassTransit retry/error handling
    }
}
```


### Structured Logging

```csharp
builder.Host.UseSerilog((context, _, loggerConfiguration) => 
    loggerConfiguration.ReadFrom.Configuration(context.Configuration).EnableSensitiveDataMasking());
```

### Dependency Injection

- Register all domain services, repositories, and infrastructure services via extension methods
- Organize service registration by domain capability
- Use scoped lifetime for domain services and repositories


### FastEndpoints Implementation
- Use FastEndpoints pattern for all API endpoints
- Follow the established structure: `{Feature}Endpoint.cs`, `{Feature}RequestDto.cs`, `{Feature}Validator.cs`
- Place endpoints in version-specific folders (`Domain/V{X}/{Feature}/`)
- **Always add logging**: Use `ILogger<T>` in endpoints and log key actions, errors, and request/response details as appropriate.
- **Always add caching from common CacheService**: Integrate the caching layer using `ICacheService<T>` for relevant endpoints. Use region-based cache keys and follow the caching strategy outlined below.
- **Endpoint configuration:**
  - Always use `DontCatchExceptions();` in `Configure()`.
    - Always use `Options(x => x.WithMetadata())` and include:
        - `.Produces<YourResponseDto>()`
        - `.Produces(StatusCodes.Status200OK)`
        - `.Produces(StatusCodes.Status304NotModified)`
        - `.Produces(StatusCodes.Status500InternalServerError)`
    - Update the response type and status codes in these statements as needed, based on the actual response of the endpoint.
- **API Path Convention:**
  - Do **not** generate full versioned API paths (e.g., `/v5/vehicles/data/{id}`).
  - Use relative resource paths (e.g., `/data/{id}`) and let the API versioning system handle the version prefix.
- Configure endpoints with proper versioning, caching, and error handling as described above.

Example endpoint structure:
```csharp
public class RetrieveDataEndpoint : Endpoint<RetrieveDataRequestDto, DataResponseModel>
{
    private readonly ILogger<RetrieveDataEndpoint> _logger;
    private readonly ICacheService<DataResponseModel> _cacheService;

    public RetrieveDataEndpoint(ILogger<RetrieveDataEndpoint> logger, ICacheService<DataResponseModel> cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public override void Configure()
    {
        Get("/data/{id}"); // Use relative path only
        Version(5);
        DontCatchExceptions();
        Options(x => x.WithMetadata(new System.Diagnostics.ActivityAttribute())
            .Produces<DataResponseModel>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(RetrieveDataRequestDto req, CancellationToken ct)
    {
        _logger.LogInformation("Handling RetrieveDataEndpoint request: {@Request}", req);
        var cacheKey = $"RetrieveData_{req.Id}";
        var response = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            // ... fetch and map data ...
            return new DataResponseModel();
        });
        await SendAsync(response);
    }
}
```

### API Versioning Strategy
- Use semantic versioning (V1, V2, V3, etc.)
- Version endpoints using `Version(X)` in FastEndpoints configuration

### Entity and Repository Pattern
- All entities inherit from `BaseEntity` from `Peddle.Foundation.Data.Repositories`
- Repository interfaces inherit from `IRepository<TEntity>` where TEntity is your domain entity
- Repository implementations inherit from `Repository<TEntity>` and implement your custom interface
- Use constructor pattern: `Repository<TEntity>(DbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)`
The following async/await data access methods are already implemented in the parent class `Repository` in `Peddle.Foundation.Data.Repositories`:
    - `GetAsync(Expression<Func<TEntity, bool>> predicate)` - Single entity retrieval with predicate
    - `GetListAsync(Expression<Func<TEntity, bool>> predicate)` - Multiple entities with predicate
    - `GetListAsync()` - All entities
    - `Queryable(Expression<Func<TEntity, bool>> predicate)` - For complex LINQ queries
    - `Add(TEntity entity)` - Add single entity
    - `AddRange(IEnumerable<TEntity> entities)` - Add multiple entities
    - `Update(TEntity entity)` - Update single entity
    - `Remove(TEntity entity)` - Remove single entity
    - `SaveChangesAsync()` - Save without audit trail
    - `SaveChangesAsync(string userId)` - Save with audit trail logging
Always reuse these existing methods in inherited repositories wherever possible, instead of creating new methods.
- Use Entity Framework Core with appropriate database provider
- Use `ExecuteBulkUpdateAsync()` and `ExecuteBulkDeleteAsync()` for bulk operations instead of deprecated `UpdateRange()` and `RemoveRange()`
- Always ignore `CreatedDateTime` and `LastModifiedDateTime` in favor of `CreatedAt`/`LastModifiedAt`.

Entity Configuration:
```csharp
// If database doesn't contain BaseEntity properties, ignore them in entity configuration
public class EntityConfiguration : IEntityTypeConfiguration<EntityName>
{
    public void Configure(EntityTypeBuilder<EntityName> builder)
    {
        builder.Ignore(x => x.CreatedDateTime);
        builder.Ignore(x => x.LastModifiedDateTime);
        // Only map CreatedAt and LastModifiedAt if they exist in database
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
        builder.Property(e => e.LastModifiedAt).HasColumnName("LastModifiedAt");
    }
}
```

Example implementation:
```csharp
public interface IDataRepository : IRepository<DataEntity>
{
    Task<IEnumerable<DataEntity>> GetDataByTypeAsync(string type);
    Task<IReadOnlyList<int>> GetUniqueYearsAsync();
}

public class DataRepository(DatabaseContext dbContext, ILoggerFactory loggerFactory)
    : Repository<DataEntity>(dbContext, loggerFactory), IDataRepository
{
    private static readonly ActivitySource ActivitySource = new("Peddle.Data.Repository");

    public async Task<IEnumerable<DataEntity>> GetDataByTypeAsync(string type)
    {
        using var activity = ActivitySource.StartActivity("GetDataByType");
        activity?.SetTag("data.type", type);
        return await GetListAsync(d => d.Type.Equals(type));
    }
    
    public async Task<IReadOnlyList<int>> GetUniqueYearsAsync()
    {
        using var activity = ActivitySource.StartActivity("GetUniqueYears");
        return await Queryable(d => true)
            .Select(d => d.Year)
            .Distinct()
            .OrderByDescending(year => year)
            .ToListAsync();
    }
}
```

### Configuration Options Pattern
- **IOptionsMonitor<T>**: Use for real-time configuration updates (singleton services)
- **IOptionsSnapshot<T>**: Use for per-request configuration (scoped services, handlers)
- **IOptions<T>**: Use for configuration that rarely changes (singleton)
- **Configuration Location**: Place configuration classes in `src/Api/Domain/Dtos/Configurations/`
- **Registration**: Always use `services.Configure<T>(configuration.GetSection("SectionPath"))`

## Code Standards and Best Practices

### Code Modification Guidelines

#### Minimal Code Changes Principle
- **Focus on Specific Requirements**: Only modify code that directly relates to the user's specific request
- **Avoid Unnecessary Updates**: Do not update existing working code logic unless explicitly requested
- **Preserve Existing Patterns**: Maintain existing code patterns and conventions when adding new features
- **Targeted Modifications**: Make surgical changes rather than broad refactoring unless required
- **Existing Logic Preservation**: When adding new functionality, preserve existing business logic and workflows
- **Version Control Friendly**: Keep changes minimal to maintain clear commit history and easier code reviews

#### When to Modify Existing Code
- **Explicitly Requested Changes**: User specifically asks to update existing functionality
- **Breaking Changes Required**: New feature requires modifications to existing interfaces or contracts
- **Bug Fixes**: Correcting identified issues in existing code
- **Integration Points**: Necessary changes to connect new features with existing systems
- **Security or Performance Issues**: Critical updates for security or performance improvements

#### When NOT to Modify Existing Code
- **Working Logic**: Existing code that functions correctly and meets requirements
- **Style Preferences**: Personal coding style differences that don't affect functionality
- **Minor Optimizations**: Small performance improvements that don't address specific issues
- **Refactoring for Refactoring**: Changes that don't add business value or fix problems
- **Unrelated Code**: Code outside the scope of the current request or feature

### API Design Patterns
- Follow RESTful principles for resource naming
- Use appropriate HTTP verbs (GET for retrieval, POST for creation)
- Implement proper status code responses (200, 400, 403, 500)
- Use DTOs for request/response models, separate from entities

### Naming Conventions

- **Consumers**: `{CommandName}CommandConsumer.cs` or `{EventName}EventConsumer.cs` (e.g., `ProcessDataCommandConsumer.cs`, `ExternalSystemEventConsumer.cs`)
- **Domain Services**: `I{ServiceName}.cs` interface with `{ServiceName}.cs` implementation
- **Repositories**: `I{EntityName}Repository.cs` interface with `{EntityName}Repository.cs` implementation
- **Entities**: `{EntityName}Entity.cs` for database entities
- **Configuration**: `{ServiceName}Configuration.cs` for service-specific configurations
- **DTOs**: `{EntityName}Dto.cs` for data transfer objects
- **Wrappers**: `I{ServiceName}Wrapper.cs` for external service abstractions
- **Mapping Profiles**: `{FeatureName}MappingProfile.cs` for AutoMapper profiles
- Use meaningful names reflecting business intent and domain concepts
- Use PascalCase for public members, camelCase for private fields

### Domain-Driven Design Patterns

- **Bounded Contexts**: Organize code by business capabilities (e.g., ProcessingFeature, IntegrationFeature)
- **Domain Services**: Encapsulate business logic that doesn't naturally fit in entities
- **Repository Pattern**: Implement domain-specific data access interfaces
- **Command/Event Processing**: Commands for internal operations, Events for external integration
- **External Service Integration**: Use wrapper pattern for external dependencies
- **Value Objects**: Use record types for immutable data structures

### Message Processing Design Patterns

- **Command Processing**: Handle internal business operations with clear workflows and state transitions
- **Event Processing**: Handle external system integration with idempotent and fault-tolerant operations  
- **Workflow Management**: Use ISender to orchestrate business workflows and process coordination
- **State Management**: Use appropriate state management patterns for complex business processes
- **Error Isolation**: Implement error handling that prevents cascade failures across processing stages
- **Data Transformation**: Apply business rules and transformations according to domain requirements

### Health Monitoring and Observability
- Configure health checks using TinyHealthCheck for service liveness
- Implement structured logging with correlation IDs for pipeline tracking
- Log start/end of each pipeline stage with relevant business context
- Monitor data transformation metrics and external service call success rates

### AutoMapper Integration Patterns

#### Domain-Specific Mapping Profiles
- **Feature-Level Profiles**: Create `{FeatureName}MappingProfile.cs` in each domain feature folder
- **Profile Registration**: AutoMapper profiles are automatically discovered via assembly scanning in `DependencyInjectionExtension.cs`
- **Naming Convention**: Use `{FeatureName}MappingProfile` naming pattern (e.g., `ProcessingFeatureMappingProfile.cs`)
- **Domain-Specific Mappings**: Keep mapping logic focused on specific domain transformations
- **Profile Inheritance**: Inherit from `Profile` class and configure mappings in constructor

#### AutoMapper Configuration Pattern
```csharp
private static IServiceCollection RegisterMapper(this IServiceCollection services)
{
    var mapperConfig = new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetExecutingAssembly()); });
    var mapper = mapperConfig.CreateMapper();
    services.AddSingleton(mapper);
    return services;
}
```

#### Example Mapping Profile Structure
```csharp
public class ProcessingFeatureMappingProfile : Profile
{
    public ProcessingFeatureMappingProfile()
    {
        CreateMap<SourceEntity, TargetEntity>()
            .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.SourceProperty))
            .ForMember(dest => dest.CalculatedField, opt => opt.MapFrom(src => CalculateValue(src)));
    }
    
    private static string CalculateValue(SourceEntity source)
    {
        // Domain-specific calculation logic
        return source.Value?.ToString() ?? "Default";
    }
}
```

### Data Processing Best Practices
- **Data Consistency**: Ensure data consistency across processing stages and external systems
- **Business Validation**: Validate data according to business rules and domain constraints
- **Bulk Operations**: Optimize database operations for performance with large data sets
- **Memory Management**: Process large datasets efficiently to avoid memory issues
- **Data Transformation**: Apply transformations according to business requirements and domain models
- **Mapping Validation**: Use AutoMapper's configuration validation to ensure all mappings are properly configured

### Testing Strategy

#### Core Testing Principles
- Test Command and Event Consumers using mock message contexts and verify processing logic
- Test Domain Services in isolation with mock dependencies
- Mock external services and systems using appropriate test doubles
- Test data transformation and mapping logic with representative data samples
- Maintain coverage for business logic and external service integration
- Organize tests by domain feature matching the source structure
- Test error scenarios and failure handling mechanisms
- Verify message processing workflows and business rule enforcement

#### AutoMapper Testing Requirements
- **Mandatory Profile Registration**: All new AutoMapper profiles MUST be added to `AutoMapperTest.cs` in `tests/UnitTests/Helper/`
- **Profile Validation**: Each mapping profile must be included in the test configuration to ensure proper mapping validation
- **Test Configuration Pattern**: Add new profiles using `cfg.AddProfile(new YourMappingProfile());` in the test setup
- **Mapping Verification**: Test mapping configurations are validated automatically during test execution
- **Profile Completeness**: Ensure all domain mapping profiles are registered in test configuration to prevent runtime mapping errors

#### Example AutoMapper Test Registration
```csharp
// In AutoMapperTest.cs constructor
private AutoMapperTest()    
{
    var mapperConfig = new MapperConfiguration(cfg =>
    {
        // Existing profiles
        cfg.AddProfile(new ProcessingFeatureMappingProfile());
        cfg.AddProfile(new IntegrationFeatureMappingProfile());
        
        // Add new profile here when created
        cfg.AddProfile(new YourNewFeatureMappingProfile());
    });
    
    Mapper = mapperConfig.CreateMapper();
}
```

## Development Workflow

### Adding a New Message Consumer

1. **Create Domain Folder**: Create a new folder under `src/Domain/` for the business capability
2. **Implement Consumer**: Create command or event consumer class implementing `IConsumer<TMessage>`
3. **Add Domain Logic**: Implement business logic directly in consumer or create domain services
4. **Create Mapping Profile**: Add `{FeatureName}MappingProfile.cs` if data transformation is needed
5. **Register Consumer**: Add consumer registration in `DependencyInjectionExtension.cs`
6. **Update Workflow**: Modify related consumers to integrate with new processing step (only if required for workflow)
7. **Add Error Handling**: Implement comprehensive error handling and logging. Follow Serilog best practices for logging as outlined in [Serilog Dos and Don'ts](https://esg.dev/posts/serilog-dos-and-donts/)
8. **Write Tests**: Create unit tests for consumer and domain logic
9. **Update Test AutoMapper**: Add mapping profile to test AutoMapper configuration if created

### Adding a New Domain Service

1. **Create Interface**: Define service interface in appropriate domain folder
2. **Implement Service**: Create service implementation with business logic
3. **Add Dependencies**: Inject required repositories and external services
4. **Create Mapping Profile**: Add `{FeatureName}MappingProfile.cs` if data transformation is needed
5. **Register Service**: Add service registration in dependency injection
6. **Add Configuration**: Create configuration classes if external integration is required
7. **Write Tests**: Create unit tests and integration tests
8. **Update Test AutoMapper**: Add mapping profile to test AutoMapper configuration if created

### External Service Integration

1. **Create Wrapper Interface**: Define wrapper interface for external service
2. **Implement Wrapper**: Create wrapper implementation with error handling
3. **Add Configuration**: Create configuration classes for service-specific settings
4. **Register Services**: Add wrapper and configuration registration in DI
5. **Add Retry Logic**: Implement appropriate retry strategies for external calls
6. **Add Integration Tests**: Create tests for external service integration

### Database Integration Updates

1. **Update Entity**: Modify or create entity classes in `DatabaseIntegration` folder
2. **Update Repository**: Add new methods to repository interfaces and implementations
3. **Update DbContext**: Add new DbSets or modify existing ones
4. **Create Migration**: Generate and apply EF Core migrations
5. **Update Tests**: Add tests for new repository methods and database operations

## Integration & Monitoring

### External Dependencies

- **External APIs**: Third-party system integration and data synchronization
- **Database Systems**: Business data persistence and querying
- **Message Broker**: RabbitMQ via MassTransit with Peddle.Foundation.Messagebroker
- **Configuration Management**: AWS SSM for configuration and secrets
- **Logging**: Serilog for structured logging and observability

### Message Flow Architecture

- **Initiation**: External events or internal triggers start business processes
- **Business Processing**: Sequential or parallel processing based on business requirements
- **External Integration**: Synchronization with external systems and services
- **Data Persistence**: Storage of business data and state management
- **Error Handling**: Failed messages handled with appropriate retry logic and error reporting

### Service Communication

- Include correlation IDs for distributed tracing across business processes
- Apply retry and timeout policies for external services and databases
- Gracefully handle service failures with circuit breaker patterns where appropriate
- Log all external service calls with appropriate detail and timing metrics
- Use structured logging for better observability and debugging

---

**All Peddle microservices must adhere to these Domain-Driven Design patterns to ensure consistency, reliability, and maintainability across the platform.**