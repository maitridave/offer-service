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
builder.Services.SwaggerDocument();

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

// Configure Swagger
app.UseSwaggerGen();

// Configure FastEndpoints
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

app.Run();