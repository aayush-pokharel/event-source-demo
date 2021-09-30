using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.core.Utility;
using event_sourcing_demo.cart.domain.Commands;
using event_sourcing_demo.cart.domain.Events;
using event_sourcing_demo.cart.domain.Exceptions;
using event_sourcing_demo.cart.domain.Interfaces;
using event_sourcing_demo.cart.domain.Services;
using event_sourcing_demo.web.api.EventHandlers;
using event_sourcing_demo.web.api.Registries;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ICurrencyConverter, FakeCurrencyConverter>();

            services.AddSingleton<IEventSerializer>(new JsonEventSerializer(new[]
            {
                typeof(CartCreated).Assembly
            })).AddInfrastructure(this.Configuration);

            services.AddScoped<ServiceFactory>(ctx => ctx.GetRequiredService);
            services.AddScoped<IMediator, Mediator>();

            services.Scan(scan =>
            {
                scan.FromAssembliesOf(typeof(CreateCart))
                    .RegisterHandlers(typeof(IRequestHandler<>))
                    .RegisterHandlers(typeof(IRequestHandler<,>))
                    .RegisterHandlers(typeof(INotificationHandler<>));
            });

            services.Decorate(typeof(INotificationHandler<>), typeof(RetryDecorator<>));

            services.AddProblemDetails(opts =>
            {
                opts.IncludeExceptionDetails = (ctx, ex) =>
                {
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsDevelopment() || env.IsStaging();
                };

                opts.MapToStatusCode<ArgumentOutOfRangeException>((int)HttpStatusCode.BadRequest);
                opts.MapToStatusCode<ValidationException>((int)HttpStatusCode.BadRequest);
                opts.MapToStatusCode<CartLineItemTransactionException>((int)HttpStatusCode.BadRequest);
            });

#if OnPremise
            services.RegisterWorker(this.Configuration);
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
