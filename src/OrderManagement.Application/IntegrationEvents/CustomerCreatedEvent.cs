namespace OrderManagement.Application.IntegrationEvents;

public record CustomerCreatedEvent
{
    public Guid CustomerId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}