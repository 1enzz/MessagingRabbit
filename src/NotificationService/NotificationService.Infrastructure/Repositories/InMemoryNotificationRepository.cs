using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Repositories;

public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly List<Notification> _notifications = [];

    public IEnumerable<Notification> GetAll() => _notifications;

    public Notification? GetById(Guid id) => _notifications.FirstOrDefault(n => n.Id == id);

    public void Add(Notification notification) => _notifications.Add(notification);

    public void Update(Notification notification)
    {
        var index = _notifications.FindIndex(n => n.Id == notification.Id);
        if (index >= 0) _notifications[index] = notification;
    }
}
