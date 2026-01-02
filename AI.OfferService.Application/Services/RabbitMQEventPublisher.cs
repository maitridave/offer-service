using MassTransit;
using Microsoft.Extensions.Logging;

namespace AI.OfferService.Application.Services;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventPublisher> _logger;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint, ILogger<MassTransitEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T eventMessage) where T : class
    {
        try
        {
            await _publishEndpoint.Publish(eventMessage);
            _logger.LogInformation("Published {EventType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {EventType}", typeof(T).Name);
            throw;
        }
    }
}