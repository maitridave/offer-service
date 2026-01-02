using FastEndpoints;
using FastEndpoints.Swagger;
using AI.OfferService.Domain.Data;
using AI.OfferService.Domain.Repositories;
using AI.OfferService.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add FastEndpoints
builder.Services.AddFastEndpoints();

// Add FastEndpoints Swagger
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "Offer Service API";
        s.Version = "v1.0";
        s.Description = "API for managing vehicle offers";
    };
});

// Add DbContext
builder.Services.AddDbContext<OfferDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// Register Event Publisher (use NoOp for local development, RabbitMQ for production)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEventPublisher, NoOpEventPublisher>();
}
else
{
    builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();
}

var app = builder.Build();

// Configure FastEndpoints (this must be before Swagger)
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    c.Serializer.Options.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    c.Endpoints.RoutePrefix = "api";
    c.Endpoints.Configurator = ep =>
    {
        ep.AllowAnonymous();
    };
});

// Configure FastEndpoints Swagger UI
app.UseSwaggerGen(); // This generates the swagger docs and serves the UI

app.Run();