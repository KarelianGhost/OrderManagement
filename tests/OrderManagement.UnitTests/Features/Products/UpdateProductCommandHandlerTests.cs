using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Products.Commands;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Products;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IRepository<Product>> _productRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly IMapper _mapper;

    public UpdateProductCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ExistingProduct_UpdatesAndInvalidatesCache()
    {
        // Arrange
        var product = new Product("Old", "Desc", 50m, 5);
        _productRepoMock
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var command = new UpdateProductCommand(product.Id, "New", "NewDesc", 75m, 10);
        var handler = new UpdateProductCommandHandler(
            _productRepoMock.Object, _unitOfWorkMock.Object, _mapper, _cacheMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be("New");
        product.Price.Should().Be(75m);
        product.StockQuantity.Should().Be(10);

        _cacheMock.Verify(c => c.RemoveAsync("products_all", It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        _productRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new UpdateProductCommand(Guid.NewGuid(), "A", "B", 1m, 1);
        var handler = new UpdateProductCommandHandler(
            _productRepoMock.Object, _unitOfWorkMock.Object, _mapper, _cacheMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NotFound");
        _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}