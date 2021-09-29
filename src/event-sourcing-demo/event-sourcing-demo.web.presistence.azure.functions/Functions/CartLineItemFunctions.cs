using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.functions.Functions
{
    public class CartLineItemFunctions : BaseFunction
    {
        public CartLineItemFunctions(IEventSerializer eventSerializer, IMediator mediator) : base(eventSerializer, mediator)
        {
        }

        [FunctionName(nameof(CartLineItemCreated))]
        public async Task CartLineItemCreated([ServiceBusTrigger("aggregate-cart-line-item", "created", Connection = "AzureWebJobsServiceBus")] Message msg)
        {
            await HandleMessage(msg);
        }
    }
}
