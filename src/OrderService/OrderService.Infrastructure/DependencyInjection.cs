using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    { 
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });
        return services;
    }
}
