using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Infrastructure.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];

    public IEnumerable<Order> GetAll() => _orders;

    public Order? GetById(Guid id) => _orders.FirstOrDefault(o => o.Id == id);

    public void Add(Order order) => _orders.Add(order);

    public void Update(Order order)
    {
        var index = _orders.FindIndex(o => o.Id == order.Id);
        if (index >= 0) _orders[index] = order;
    }
}
