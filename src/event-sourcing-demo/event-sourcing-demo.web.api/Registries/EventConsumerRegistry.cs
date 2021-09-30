using event_sourcing_demo.cart.core.EventBus;
using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.web.api.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.api.Registries
{
    public static class EventConsumerRegistry
    {
        public static IServiceCollection RegisterWorker(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IEventConsumerFactory, EventConsumerFactory>();

            services.AddHostedService(ctx =>
            {
                var factory = ctx.GetRequiredService<IEventConsumerFactory>();
                return new EventsConsumerWorker(factory);
            });

            return services;
        }
    }
}
