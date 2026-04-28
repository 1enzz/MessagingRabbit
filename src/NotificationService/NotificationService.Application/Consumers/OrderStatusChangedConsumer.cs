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
    public class OrderStatusChangedConsumer(INotificationService notificationService) : IConsumer<OrderStatusChanged>
    {
        private readonly INotificationService _notificationService = notificationService;

        public Task Consume(ConsumeContext<OrderStatusChanged> context) 
        {
            var message = $"Pedido {context.Message.OrderId} recebeu atualizações";
            var request = new CreateNotificationRequest(NotificationType.OrderStatusChanged, message, context.Message.OrderId.ToString());

             _notificationService.Create(request);

            return Task.CompletedTask;
        }
    }
}
