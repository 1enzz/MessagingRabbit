using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    IEnumerable<Notification> GetAll();
    Notification? GetById(Guid id);
    void Add(Notification notification);
    void Update(Notification notification);
}
