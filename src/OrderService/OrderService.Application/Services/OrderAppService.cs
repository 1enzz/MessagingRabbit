using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using Shared.Contracts.Messages;

namespace OrderService.Application.Services;

public class OrderAppService(IOrderRepository repository, IMessagePublisher publisher) : IOrderService
{
    public IEnumerable<OrderResponse> GetAll() =>
        repository.GetAll().Select(ToResponse);

    public OrderResponse? GetById(Guid id)
    {
        var order = repository.GetById(id);
        return order is null ? null : ToResponse(order);
    }

    public async Task<OrderResponse> Create(CreateOrderRequest request)
    {
        var items = request.Items
            .Select(i => new OrderItem { ProductName = i.ProductName, Quantity = i.Quantity, UnitPrice = i.UnitPrice })
            .ToList();

        var order = Order.Create(request.CustomerName, items);
        repository.Add(order);

        await publisher.PublishOrderCreated(
            new OrderCreated
            {
                OrderId = order.Id,
                CustomerName = request.CustomerName,
                CreatedOn = DateTime.Now,
                TotalOrder = order.TotalAmount
            });
        return ToResponse(order);
    }

    public async Task<OrderResponse?> UpdateStatus(Guid id, UpdateOrderStatusRequest request)
    {
        var order = repository.GetById(id);
        if (order is null) return null;

        order.UpdateStatus(request.Status);
        repository.Update(order);

        await publisher.SendOrderStatusChanged(
            new OrderStatusChanged
            {
                OrderId = id,
                OrderStatus = request.Status.ToString(),
                ModifiedOn = DateTime.Now
            });

        return ToResponse(order);
    }

    private static OrderResponse ToResponse(Order order) =>
        new(
            order.Id,
            order.CustomerName,
            order.Items.Select(i => new OrderItemDto(i.ProductName, i.Quantity, i.UnitPrice)).ToList(),
            order.TotalAmount,
            order.Status,
            order.CreatedAt);
}
