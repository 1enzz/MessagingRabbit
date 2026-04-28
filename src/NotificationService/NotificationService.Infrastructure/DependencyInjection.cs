using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Repositories;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
        return services;
    }
}
