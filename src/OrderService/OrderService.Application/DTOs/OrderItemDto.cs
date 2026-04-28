namespace OrderService.Application.DTOs;

public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice);
