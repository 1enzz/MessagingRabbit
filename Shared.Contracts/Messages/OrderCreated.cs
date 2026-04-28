
namespace Shared.Contracts.Messages
{
    public record OrderCreated
    {
        public Guid OrderId { get; init; }
        public DateTime CreatedOn { get; init; }
        public string CustomerName { get; init; }
        public decimal TotalOrder { get; init; }
    }
}
