using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs;

public record UpdateOrderStatusRequest(OrderStatus Status);
