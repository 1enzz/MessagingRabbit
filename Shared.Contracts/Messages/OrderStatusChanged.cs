
namespace Shared.Contracts.Messages
{
    public record OrderStatusChanged
    {
        public Guid OrderId { get; init; }
        public string OrderStatus { get; init; }
        public DateTime ModifiedOn { get; init; }
    }
}
