using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Order Service", Version = "v1" });
});
builder.Services.AddSingleton<OrderRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service v1"));
}

app.UseHttpsRedirection();

// ── Orders ──────────────────────────────────────────────────────────────────

app.MapGet("/orders", (OrderRepository repo) => repo.GetAll())
    .WithName("GetOrders");

app.MapGet("/orders/{id:guid}", (Guid id, OrderRepository repo) =>
{
    var order = repo.GetById(id);
    return order is null ? Results.NotFound() : Results.Ok(order);
})
.WithName("GetOrderById");

app.MapPost("/orders", ([FromBody] CreateOrderRequest request, OrderRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerName) || request.Items.Count == 0)
        return Results.BadRequest("CustomerName and at least one item are required.");

    var order = new Order
    {
        Id = Guid.NewGuid(),
        CustomerName = request.CustomerName,
        Items = request.Items,
        TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    repo.Add(order);

    // Futuramente: publicar evento "OrderCreated" no broker de mensagens
    return Results.Created($"/orders/{order.Id}", order);
})
.WithName("CreateOrder");

app.MapPatch("/orders/{id:guid}/status", (Guid id, [FromBody] UpdateStatusRequest request, OrderRepository repo) =>
{
    var order = repo.GetById(id);
    if (order is null) return Results.NotFound();

    order.Status = request.Status;

    // Futuramente: publicar evento "OrderStatusChanged" no broker de mensagens
    return Results.Ok(order);
})
.WithName("UpdateOrderStatus");

app.Run();

// ── Models ───────────────────────────────────────────────────────────────────

public class Order
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }

public record CreateOrderRequest(string CustomerName, List<OrderItem> Items);
public record UpdateStatusRequest(OrderStatus Status);

// ── Repository ───────────────────────────────────────────────────────────────

public class OrderRepository
{
    private readonly List<Order> _orders = [];

    public IEnumerable<Order> GetAll() => _orders;
    public Order? GetById(Guid id) => _orders.FirstOrDefault(o => o.Id == id);
    public void Add(Order order) => _orders.Add(order);
}
