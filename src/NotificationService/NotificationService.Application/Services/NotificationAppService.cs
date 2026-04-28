using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;

public class NotificationAppService(INotificationRepository repository) : INotificationService
{
    public IEnumerable<NotificationResponse> GetAll() =>
        repository.GetAll().Select(ToResponse);

    public NotificationResponse? GetById(Guid id)
    {
        var notification = repository.GetById(id);
        return notification is null ? null : ToResponse(notification);
    }

    public NotificationResponse Create(CreateNotificationRequest request)
    {
        var notification = Notification.Create(request.Type, request.Message, request.Recipient);
        repository.Add(notification);

        // Futuramente: este método será chamado pelo consumer do broker de mensagens
        return ToResponse(notification);
    }

    public NotificationResponse? MarkAsRead(Guid id)
    {
        var notification = repository.GetById(id);
        if (notification is null) return null;

        notification.MarkAsRead();
        repository.Update(notification);
        return ToResponse(notification);
    }

    private static NotificationResponse ToResponse(Domain.Entities.Notification n) =>
        new(n.Id, n.Type, n.Message, n.Recipient, n.CreatedAt, n.IsRead);
}
