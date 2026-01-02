using AI.OfferService.Application.Services;
using Microsoft.Extensions.Logging;

namespace AI.OfferService.Application.Services;

public class NoOpEventPublisher : IEventPublisher
{
    private readonly ILogger<NoOpEventPublisher> _logger;

    public NoOpEventPublisher(ILogger<NoOpEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T eventMessage, string routingKey) where T : class
    {
        _logger.LogInformation("Event publishing disabled. Would have published {EventType} with routing key {RoutingKey}", 
            typeof(T).Name, routingKey);
        return Task.CompletedTask;
    }
}