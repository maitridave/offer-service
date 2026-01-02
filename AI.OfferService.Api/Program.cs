using FastEndpoints;
using FastEndpoints.Swagger;
using AI.OfferService.Domain.Data;
using AI.OfferService.Domain.Repositories;
using AI.OfferService.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;

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

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        // Custom exchange naming
        cfg.Message<AI.OfferService.Application.Events.OfferCreatedEvent>(e => e.SetEntityName("automotive.offer.created"));
        cfg.Message<AI.OfferService.Application.Events.OfferUpdatedEvent>(e => e.SetEntityName("automotive.offer.updated"));

        cfg.ConfigureEndpoints(context);
    });
});

// Register Event Publisher (use NoOp for local development, MassTransit for production)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEventPublisher, NoOpEventPublisher>();
}
else
{
    builder.Services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
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