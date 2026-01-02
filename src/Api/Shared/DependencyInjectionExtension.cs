using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AI.OfferService.Domain.OfferFeature;
using AI.OfferService.Models;
using Microsoft.EntityFrameworkCore;

namespace AI.OfferService.Shared
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddServiceExtension(this IServiceCollection services,
            IConfiguration configuration)
        {
            var mapperConfig = new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetExecutingAssembly()); });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Register AicodeChallengeDbContext with SQL Server (Database First)
            services.AddDbContext<AicodeChallengeDbContext>(options =>
                options.UseSqlServer("Data Source=192.168.2.97\\MSSQLSERVER;Initial Catalog=AICodeChallenge2025;User ID=remote_user;Password=xi(w1rMbvhMA;"));

            services.RegisterCorrelationIdServices();
            services.RegisterHealthCheck();

            // Register OfferFeature services and repositories
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IOfferService, Domain.OfferFeature.OfferService>();
            // Register IEventPublisher
            services.AddScoped<IEventPublisher, EventPublisher>();

            return services;
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Set the JSON serializer options
            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
            });

            // Set the API Behavior options
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });

            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration.GetValue<string>("IDENTITY_PROVIDER_SERVICE_HOST");
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = "universal" // Replace Peddle constant with string
                    };
                });
            return services;
        }

        private static void RegisterCorrelationIdServices(this IServiceCollection services)
        {
            services.AddCorrelationId(options =>
            {
                options.AddToLoggingScope = true;
                options.RequestHeader = "X-Correlation-ID"; // Replace Peddle constant with string
                options.UpdateTraceIdentifier = true;
            }).WithGuidProvider();
        }

        private static void RegisterHealthCheck(this IServiceCollection services)
        {
            services.AddHealthChecks();
        }

    }
}