using System;
using Convey;
using Convey.Persistence.MongoDB;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Parcels.Application;
using Pacco.Services.Parcels.Application.Commands;
using Pacco.Services.Parcels.Application.Events.External;
using Pacco.Services.Parcels.Application.Services;
using Pacco.Services.Parcels.Core.Repositories;
using Pacco.Services.Parcels.Infrastructure.Exceptions;
using Pacco.Services.Parcels.Infrastructure.Mongo.Documents;
using Pacco.Services.Parcels.Infrastructure.Mongo.Repositories;
using Pacco.Services.Parcels.Infrastructure.Services;

namespace Pacco.Services.Parcels.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
        {
            builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            builder.Services.AddTransient<IMessageBroker, MessageBroker>();
            builder.Services.AddTransient<ICustomerRepository, CustomerMongoRepository>();
            builder.Services.AddTransient<IParcelRepository, ParcelMongoRepository>();

            return builder
                .AddQueryHandlers()
                .AddInMemoryQueryDispatcher()
                .AddHttpClient()
                .AddConsul()
                .AddFabio()
                .AddRabbitMq()
                .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
                .AddMongo()
                .AddMongoRepository<CustomerDocument, Guid>("Customers")
                .AddMongoRepository<ParcelDocument, Guid>("Parcels");
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
                .UsePublicContracts<ContractAttribute>()
                .UseInitializers()
                .UseConsul()
                .UseRabbitMq()
                .SubscribeCommand<AddParcel>()
                .SubscribeCommand<DeleteParcel>()
                .SubscribeEvent<OrderCanceled>()
                .SubscribeEvent<OrderDeleted>()
                .SubscribeEvent<ParcelAddedToOrder>()
                .SubscribeEvent<ParcelDeletedFromOrder>()
                .SubscribeEvent<CustomerCreated>();

            return app;
        }
    }
}