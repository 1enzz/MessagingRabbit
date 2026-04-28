using Shared.Contracts.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishOrderCreated(OrderCreated message);
        Task SendOrderStatusChanged(OrderStatusChanged message);
    }
}
