using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces;

public interface INotificationService
{
    IEnumerable<NotificationResponse> GetAll();
    NotificationResponse? GetById(Guid id);
    NotificationResponse Create(CreateNotificationRequest request);
    NotificationResponse? MarkAsRead(Guid id);
}
