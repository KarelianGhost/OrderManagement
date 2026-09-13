using MassTransit;
using OrderManagement.Application.IntegrationEvents;

namespace OrderManagement.WebApi.Consumers;

public class CustomerCreatedEventConsumer : IConsumer<CustomerCreatedEvent>
{
    private readonly ILogger<CustomerCreatedEventConsumer> _logger;

    public CustomerCreatedEventConsumer(ILogger<CustomerCreatedEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Клиент создан: {CustomerId}, {Email}, {FullName}",
            msg.CustomerId, msg.Email, msg.FullName);
        return Task.CompletedTask;
    }
}