namespace OrderManagement.Application.IntegrationEvents;

public record ProductCreatedEvent
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
}