using OrderService.Application.DTOs;

namespace OrderService.Application.Interfaces;

public interface IOrderService
{
    IEnumerable<OrderResponse> GetAll();
    OrderResponse? GetById(Guid id);
    Task<OrderResponse> Create(CreateOrderRequest request);
    Task<OrderResponse?> UpdateStatus(Guid id, UpdateOrderStatusRequest request);
}
