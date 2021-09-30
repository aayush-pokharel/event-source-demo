﻿using event_sourcing_demo.cart.core.Events;
using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.domain;
using event_sourcing_demo.presistence.azure;
using event_sourcing_demo.web.presistence.azure.functions.EventHandlers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.api.Registries
{
    public static class InfrastructureRegistry
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration config)
        {

#if OnPremise
            services.AddOnPremiseInfrastructure(config);
            services.AddSingleton<ICustomerEmailsService>(ctx=>
            {
                var dbName = config["commandsDbName"];
                var client = ctx.GetRequiredService<MongoClient>();
                var database = client.GetDatabase(dbName);
                return new CustomerEmailsService(database);
            });
#endif

#if OnAzure
            services.AddAzureInfrastructure(config);
            services.Scan(scan =>
            {
                scan.FromAssembliesOf(typeof(CartEventsHandler))
                    .RegisterHandlers(typeof(IRequestHandler<>))
                    .RegisterHandlers(typeof(IRequestHandler<,>))
                    .RegisterHandlers(typeof(INotificationHandler<>));
            });
#endif
            return services
                .AddEventsService<Cart, Guid>()
                .AddEventsService<CartLineItem, Guid>()
                .AddEventsService<Product, Guid>();
        }

#if OnPremise

        private static IServiceCollection AddOnPremiseInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Scan(scan =>
            {
                scan.FromAssembliesOf(typeof(CustomerDetailsHandler))
                    .RegisterHandlers(typeof(IRequestHandler<>))
                    .RegisterHandlers(typeof(IRequestHandler<,>))
                    .RegisterHandlers(typeof(INotificationHandler<>));
            }).AddMongoDb(config);

            var kafkaConnStr = config.GetConnectionString("kafka");
            var eventsTopicName = config["eventsTopicName"];
            var groupName = config["eventsTopicGroupName"];
            var consumerConfig = new EventConsumerConfig(kafkaConnStr, eventsTopicName, groupName);

            var eventstoreConnStr = config.GetConnectionString("eventstore");

            return services.AddKafka(consumerConfig)
                .AddEventStore(eventstoreConnStr);
        }

#endif

        private static IServiceCollection AddEventsService<TA, TK>(this IServiceCollection services)
            where TA : class, IAggregateRoot<TK>
        {
            return services.AddSingleton<IEventsService<TA, TK>>(ctx =>
            {
                var eventsProducer = ctx.GetRequiredService<IEventProducer<TA, TK>>();
                var eventsRepo = ctx.GetRequiredService<IEventsRepository<TA, TK>>();

                return new EventsService<TA, TK>(eventsRepo, eventsProducer);
            });
        }
    }
}
