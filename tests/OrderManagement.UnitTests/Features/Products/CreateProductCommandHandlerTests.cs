using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Products.Commands;
using OrderManagement.Application.IntegrationEvents;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IRepository<Product>> _productRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly IMapper _mapper;

    public CreateProductCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessAndPublishesEvent()
    {
        // Arrange
        _productRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var command = new CreateProductCommand("Test", "Desc", 99.99m, 10);

        var handler = new CreateProductCommandHandler(
            _productRepoMock.Object,
            _unitOfWorkMock.Object,
            _mapper,
            _cacheMock.Object,
            _eventPublisherMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Test");
        result.Value.Price.Should().Be(99.99m);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync("products_all", It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(
            e => e.PublishAsync(It.IsAny<ProductCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}