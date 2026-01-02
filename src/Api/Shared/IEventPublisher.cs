using System.Threading.Tasks;

namespace AI.OfferService.Shared
{
    public interface IEventPublisher
    {
        Task Publish<TEvent>(TEvent @event) where TEvent : class;
    }
}
