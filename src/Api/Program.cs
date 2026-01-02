using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using CorrelationId;
using FastEndpoints;
using AI.OfferService.Shared;
using Serilog;

var webAppBuilder = WebApplication.CreateBuilder(args);

webAppBuilder.Services.AddEndpointsApiExplorer();
webAppBuilder.Services.AddFastEndpoints(o => o.IncludeAbstractValidators = true);

webAppBuilder.Configuration.SetBasePath(webAppBuilder.Environment.ContentRootPath).AddEnvironmentVariables();
webAppBuilder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

webAppBuilder.Logging.ClearProviders();
webAppBuilder.Host.UseSerilog((context, _, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    }
);

webAppBuilder.Services.AddControllers();

webAppBuilder.Services.AddConfigurations(webAppBuilder.Configuration)
    .AddServiceExtension(webAppBuilder.Configuration)
    .AddAuthentication(webAppBuilder.Configuration);
    
var app = webAppBuilder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCorrelationId();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    c.Serializer.Options.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    c.Versioning.Prefix = "v";
    c.Versioning.DefaultVersion = 1;
    c.Versioning.PrependToRoute = true;
});

app.MapHealthChecks("/health");

app.Run();