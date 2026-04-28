namespace OrderService.Application.DTOs;

public record CreateOrderRequest(string CustomerName, List<OrderItemDto> Items);
