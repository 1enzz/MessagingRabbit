using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;

namespace NotificationService.API.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/notifications").WithTags("Notifications");

        group.MapGet("/", (INotificationService service) => Results.Ok(service.GetAll()))
            .WithName("GetNotifications")
            .WithSummary("Lista todas as notificações");

        group.MapGet("/{id:guid}", (Guid id, INotificationService service) =>
        {
            var notification = service.GetById(id);
            return notification is null ? Results.NotFound() : Results.Ok(notification);
        })
        .WithName("GetNotificationById")
        .WithSummary("Busca notificação por ID");

        group.MapPost("/", ([FromBody] CreateNotificationRequest request, INotificationService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.Recipient) || string.IsNullOrWhiteSpace(request.Message))
                return Results.BadRequest("Recipient e Message são obrigatórios.");

            var notification = service.Create(request);
            return Results.Created($"/notifications/{notification.Id}", notification);
        })
        .WithName("CreateNotification")
        .WithSummary("Cria uma notificação manualmente");

        group.MapPatch("/{id:guid}/read", (Guid id, INotificationService service) =>
        {
            var notification = service.MarkAsRead(id);
            return notification is null ? Results.NotFound() : Results.Ok(notification);
        })
        .WithName("MarkNotificationAsRead")
        .WithSummary("Marca uma notificação como lida");
    }
}
