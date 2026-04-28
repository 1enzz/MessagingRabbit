using MassTransit;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Consumers
{
    public class OrderCreatedConsumer(INotificationService notificationService) : IConsumer<OrderCreated>
    {
        private readonly INotificationService _notificationService = notificationService;

        public Task Consume(ConsumeContext<OrderCreated> context) 
        {
            var message = $"Pedido {context.Message.OrderId} criado. Valor: {context.Message.TotalOrder}";
            var request = new CreateNotificationRequest(NotificationType.OrderCreated, message, context.Message.CustomerName);

            _notificationService.Create(request);

            return Task.CompletedTask;
            
        }
    }
}
