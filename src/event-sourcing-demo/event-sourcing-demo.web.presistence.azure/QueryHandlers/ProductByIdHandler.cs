﻿using event_sourcing_demo.presistence.azure;
using event_sourcing_demo.web.core;
using event_sourcing_demo.web.core.Queries.Models;
using MediatR;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.QueryHandlers
{
    public class ProductByIdHandler : IRequestHandler<ProductById, ProductDetails>
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public ProductByIdHandler(IDbContainerProvider containerProvider)
        {
            if (containerProvider == null)
                throw new ArgumentNullException(nameof(containerProvider));

            _container = containerProvider.GetContainer("ProductDetails");
        }

        public async Task<ProductDetails> Handle(ProductById request, CancellationToken cancellationToken)
        {
            var partitionKey = new PartitionKey(request.Id.ToString());

            //by design, ReadItemAsync() throws is item not found

            var response = await _container.ReadItemStreamAsync(request.Id.ToString(), partitionKey, cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            return _container.Database.Client.ClientOptions.Serializer.FromStream<ProductDetails>(response.Content);
        }
    }
}

