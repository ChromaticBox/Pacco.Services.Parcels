using System.Threading.Tasks;
using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Pacco.Services.Parcels.Application.Services;

namespace Pacco.Services.Parcels.Infrastructure.Services
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IBusPublisher _busPublisher;
        private readonly ICorrelationContextAccessor _contextAccessor;

        public MessageBroker(IBusPublisher busPublisher, ICorrelationContextAccessor contextAccessor)
        {
            _busPublisher = busPublisher;
            _contextAccessor = contextAccessor;
        }

        public async Task PublishAsync(params IEvent[] events)
        {
            if (events is null)
            {
                return;
            }

            foreach (var @event in events)
            {
                if (@event is null)
                {
                    continue;
                }

                await _busPublisher.PublishAsync(@event, _contextAccessor.CorrelationContext);
            }
        }
    }
}