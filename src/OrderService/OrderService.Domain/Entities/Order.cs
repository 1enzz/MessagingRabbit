using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public List<OrderItem> Items { get; private set; } = [];
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { }

    public static Order Create(string customerName, List<OrderItem> items)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName,
            Items = items,
            TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice),
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}
