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

    public Task PublishAsync<T>(T eventMessage) where T : class
    {
        _logger.LogInformation("Event publishing disabled. Would have published {EventType}", typeof(T).Name);
        return Task.CompletedTask;
    }
}