using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public NotificationType Type { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string Recipient { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }

    private Notification() { }

    public static Notification Create(NotificationType type, string message, string recipient)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            Type = type,
            Message = message,
            Recipient = recipient,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
    }

    public void MarkAsRead() => IsRead = true;
}
