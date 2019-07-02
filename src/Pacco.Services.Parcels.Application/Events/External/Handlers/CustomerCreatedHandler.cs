using System.Threading.Tasks;
using Convey.CQRS.Events;
using Pacco.Services.Parcels.Core.Entities;
using Pacco.Services.Parcels.Core.Repositories;

namespace Pacco.Services.Parcels.Application.Events.External.Handlers
{
    public class CustomerCreatedHandler : IEventHandler<CustomerCreated>
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerCreatedHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task HandleAsync(CustomerCreated @event)
        {
            if (await _customerRepository.ExistsAsync(@event.Id))
            {
                return;
            }

            await _customerRepository.AddAsync(new Customer(@event.Id));
        }
    }
}