using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.functions.Functions
{
    public class CartFunctions : BaseFunction
    {
        public CartFunctions(IEventSerializer eventSerializer, IMediator mediator) : base(eventSerializer, mediator)
        {
        }

        [FunctionName(nameof(CartCreated))]
        public async Task CartCreated([ServiceBusTrigger("aggregate-cart", "created", Connection = "AzureWebJobsServiceBus")] Message msg)
        {
            await HandleMessage(msg);
        }
    }
}
