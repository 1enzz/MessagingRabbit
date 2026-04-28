using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs;

public record CreateNotificationRequest(NotificationType Type, string Message, string Recipient);
