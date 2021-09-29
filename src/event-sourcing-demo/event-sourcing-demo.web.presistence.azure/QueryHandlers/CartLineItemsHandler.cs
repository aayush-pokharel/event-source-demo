using event_sourcing_demo.presistence.azure;
using event_sourcing_demo.web.core;
using event_sourcing_demo.web.core.Queries.Models;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.QueryHandlers
{
    public class CartLineItemsHandler : IRequestHandler<CartLineItems, IEnumerable<CartLineItemDetails>>
    {
        private readonly Container _container;

        public CartLineItemsHandler(IDbContainerProvider containerProvider)
        {
            if (containerProvider == null)
                throw new ArgumentNullException(nameof(containerProvider));

            _container = containerProvider.GetContainer("CartLineItemDetails");
        }

        public async Task<IEnumerable<CartLineItemDetails>> Handle(CartLineItems request, CancellationToken cancellationToken)
        {
            var results = new List<CartLineItemDetails>();

            var iterator = _container.GetItemLinqQueryable<CartLineItemDetails>().ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync(cancellationToken))
                {
                    results.Add(item);
                }
            }

            return results;
        }
    }
}
