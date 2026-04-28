using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Consumers;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Repositories;
using Shared.Contracts.Messages;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderStatusChangedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("order-status-changed", e =>
                {
                    e.ConfigureConsumer<OrderStatusChangedConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
