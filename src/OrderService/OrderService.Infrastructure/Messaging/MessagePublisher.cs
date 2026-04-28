using MassTransit;
using OrderService.Application.Interfaces;
using Shared.Contracts.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Infrastructure.Messaging
{
    public class MessagePublisher(IPublishEndpoint publish, ISendEndpointProvider send) : IMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint = publish;
        private readonly ISendEndpointProvider _sendEndpoint = send;

        public async Task PublishOrderCreated(OrderCreated message)
        {
            await _publishEndpoint.Publish(message);
        }

        public async Task SendOrderStatusChanged(OrderStatusChanged message) 
        {
            var endpoint = await _sendEndpoint.GetSendEndpoint(new Uri("queue:order-status-changed"));
            await endpoint.Send(message);
        }
    }
}
