using OrderManagement.Application.Interfaces;

namespace OrderManagement.IntegrationTests.Fakes;

public class NoOpEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        => Task.CompletedTask;
}