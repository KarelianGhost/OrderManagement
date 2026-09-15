using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Products.Commands;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IRepository<Product>> _productRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    [Fact]
    public async Task Handle_ExistingProduct_DeletesAndInvalidatesCache()
    {
        // Arrange
        var product = new Product("Test", "Desc", 10m, 1);
        _productRepoMock
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new DeleteProductCommandHandler(
            _productRepoMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);

        // Act
        var result = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _productRepoMock.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync("products_all", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        _productRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new DeleteProductCommandHandler(
            _productRepoMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);

        // Act
        var result = await handler.Handle(new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NotFound");
        _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}