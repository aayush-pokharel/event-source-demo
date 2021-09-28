using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.presistence.azure
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            return services.AddSingleton<CosmosClient>(ctx =>
            {
                var options = new CosmosClientOptions()
                {
                    Serializer = new CustomJsonSerializer()
                };
                var connectionString = config.GetConnectionString("cosmos");
                return new CosmosClient(connectionString, options);
            }).AddSingleton<ITopicClientFactory>(ctx =>
            {
                var connectionString = config.GetConnectionString("producer");
                return new TopicClientFactory(connectionString);
            }).AddEventsProducer<Cart, Guid>(config)
                .AddEventsProducer<Product, Guid>(config)
                .AddEventsProducer<CartLineItem, Guid>(config)
                .AddEventsRepository<Cart, Guid>(config)
                .AddEventsRepository<Product, Guid>(config)
                .AddEventsRepository<CartLineItem, Guid>(config);
        }

        private static IServiceCollection AddEventsRepository<TA, TK>(this IServiceCollection services, IConfiguration config)
            where TA : class, IAggregateRoot<TK>
        {
            return services.AddSingleton<IDbContainerProvider>(ctx =>
            {
                var cosmos = ctx.GetRequiredService<CosmosClient>();
                var dbName = config["dbName"];
                var db = cosmos.GetDatabase(dbName);
                return new DbContainerProvider(db);
            }).AddSingleton<IEventsRepository<TA, TK>>(ctx =>
            {
                var containerProvider = ctx.GetRequiredService<IDbContainerProvider>();
                var eventDeserializer = ctx.GetRequiredService<IEventSerializer>();
                return new EventsRepository<TA, TK>(containerProvider, eventDeserializer);
            });
        }

        private static IServiceCollection AddEventsProducer<TA, TK>(this IServiceCollection services, IConfiguration config)
            where TA : class, IAggregateRoot<TK>
        {
            var topicsBaseName = config["topicsBaseName"];
            return services.AddSingleton<IEventProducer<TA, TK>>(ctx =>
            {
                var clientFactory = ctx.GetRequiredService<ITopicClientFactory>();
                var eventDeserializer = ctx.GetRequiredService<IEventSerializer>();
                var logger = ctx.GetRequiredService<ILogger<EventProducer<TA, TK>>>();
                return new EventProducer<TA, TK>(clientFactory, topicsBaseName, eventDeserializer, logger);
            });
        }
    }
}
