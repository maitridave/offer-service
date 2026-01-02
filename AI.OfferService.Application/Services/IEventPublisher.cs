namespace AI.OfferService.Application.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventMessage, string routingKey) where T : class;
}