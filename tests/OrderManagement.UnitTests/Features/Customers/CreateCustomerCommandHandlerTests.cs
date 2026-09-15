using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Features.Customers.Commands;
using OrderManagement.Application.IntegrationEvents;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Customers;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithCustomerDto()
    {
        // Arrange
        var command = new CreateCustomerCommand("Иван", "Петров", "ivan@example.com", "+79161234567");

        _customerRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        var handler = new CreateCustomerCommandHandler(
            _customerRepoMock.Object,
            _unitOfWorkMock.Object,
            _mapper,
            _cacheMock.Object,
            _eventPublisherMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("ivan@example.com");
        result.Value.FirstName.Should().Be("Иван");

        // Проверяем, что SaveChanges был вызван
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Проверяем, что событие было опубликовано
        _eventPublisherMock.Verify(
            e => e.PublishAsync(It.IsAny<CustomerCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);

        // Проверяем, что кэш инвалидирован
        _cacheMock.Verify(
            c => c.RemoveAsync("customers_all", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_PropagatesException()
    {
        // Arrange
        _customerRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB failure"));

        var handler = new CreateCustomerCommandHandler(
            _customerRepoMock.Object,
            _unitOfWorkMock.Object,
            _mapper,
            _cacheMock.Object,
            _eventPublisherMock.Object);

        var command = new CreateCustomerCommand("A", "B", "a@b.com", "123");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}