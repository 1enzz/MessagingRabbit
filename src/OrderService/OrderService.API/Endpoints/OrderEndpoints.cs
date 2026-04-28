using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/", (IOrderService service) => Results.Ok(service.GetAll()))
            .WithName("GetOrders")
            .WithSummary("Lista todos os pedidos");

        group.MapGet("/{id:guid}", (Guid id, IOrderService service) =>
        {
            var order = service.GetById(id);
            return order is null ? Results.NotFound() : Results.Ok(order);
        })
        .WithName("GetOrderById")
        .WithSummary("Busca pedido por ID");

        group.MapPost("/", async ([FromBody] CreateOrderRequest request, IOrderService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName) || request.Items.Count == 0)
                return Results.BadRequest("CustomerName e ao menos um item são obrigatórios.");

            var order = await service.Create(request);
            return Results.Created($"/orders/{order.Id}", order);
        })
        .WithName("CreateOrder")
        .WithSummary("Cria um novo pedido");

        group.MapPatch("/{id:guid}/status", async (Guid id, [FromBody] UpdateOrderStatusRequest request, IOrderService service) =>
        {
            var order = await service.UpdateStatus(id, request);
            return order is null ? Results.NotFound() : Results.Ok(order);
        })
        .WithName("UpdateOrderStatus")
        .WithSummary("Atualiza o status de um pedido");
    }
}
