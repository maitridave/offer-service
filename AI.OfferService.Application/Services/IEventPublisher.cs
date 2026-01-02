using MassTransit;

namespace AI.OfferService.Application.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventMessage) where T : class;
}