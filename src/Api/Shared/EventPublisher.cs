using System.Threading.Tasks;
using MassTransit;

namespace AI.OfferService.Shared
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Publish<TEvent>(TEvent @event) where TEvent : class
        {
            await _publishEndpoint.Publish(@event);
        }
    }
}
