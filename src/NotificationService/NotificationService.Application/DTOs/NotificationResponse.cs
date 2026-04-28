using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs;

public record NotificationResponse(
    Guid Id,
    NotificationType Type,
    string Message,
    string Recipient,
    DateTime CreatedAt,
    bool IsRead);
