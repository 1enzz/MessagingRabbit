using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs;

public record OrderResponse(
    Guid Id,
    string CustomerName,
    List<OrderItemDto> Items,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt);
