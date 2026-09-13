using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.IntegrationEvents;

namespace OrderManagement.WebApi.Consumers;

public class ProductCreatedEventConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventConsumer> _logger;

    public ProductCreatedEventConsumer(ILogger<ProductCreatedEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Продукт создан: {ProductId}, {Name}, {Price}",
            message.ProductId, message.Name, message.Price);

        // Здесь можно отправить email, обновить отчёты и т.д.
        return Task.CompletedTask;
    }
}