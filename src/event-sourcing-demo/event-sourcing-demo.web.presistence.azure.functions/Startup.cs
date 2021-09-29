using System;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using event_sourcing_demo.cart.domain.Events;
using event_sourcing_demo.presistence.azure;
using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.core.Utility;
using event_sourcing_demo.web.presistence.azure.Functions;
using event_sourcing_demo.web.presistence.azure.functions.EventHandlers;

[assembly: FunctionsStartup(typeof(Startup))]
namespace event_sourcing_demo.web.presistence.azure.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IEventSerializer>(new JsonEventSerializer(new[]
            {
                typeof(CartCreated).Assembly
            }));

            builder.Services.AddSingleton<CosmosClient>(ctx =>
            {

                var options = new CosmosClientOptions()
                {
                    Serializer = new CustomJsonSerializer()
                };
                var connectionString = Environment.GetEnvironmentVariable("cosmos");
                return new CosmosClient(connectionString, options);
            });
            builder.Services.AddSingleton<IDbContainerProvider>(ctx =>
            {
                var cosmos = ctx.GetRequiredService<CosmosClient>();
                var dbName = Environment.GetEnvironmentVariable("cosmosDbName");
                var db = cosmos.GetDatabase(dbName);
                return new DbContainerProvider(db);
            });

            builder.Services.AddMediatR(typeof(CartLineItemsEventsHandler));
        }
    }
}