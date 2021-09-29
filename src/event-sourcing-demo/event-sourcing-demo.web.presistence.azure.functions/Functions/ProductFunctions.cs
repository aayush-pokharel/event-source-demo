using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.domain.Events;
using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.functions.Functions
{
    public class ProductFunctions : BaseFunction
    {
        public ProductFunctions(IEventSerializer eventSerializer, IMediator mediator) : base(eventSerializer, mediator)
        {
        }

        [FunctionName(nameof(ProductCreated))]
        public async Task ProductCreated([ServiceBusTrigger("aggregate-product", "created", Connection = "AzureWebJobsServiceBus")] Message msg)
        {
            await HandleMessage(msg);
        }
    }
}
