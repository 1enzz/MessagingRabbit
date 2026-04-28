using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces;

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();
    Order? GetById(Guid id);
    void Add(Order order);
    void Update(Order order);
}
