using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Products.Queries;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Products;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IRepository<Product>> _productRepoMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedDataWithoutDbCall()
    {
        // Arrange
        var cached = new List<OrderManagement.Application.DTOs.ProductDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Cached", Price = 100m, StockQuantity = 5 }
        };
        _cacheMock
            .Setup(c => c.GetAsync<IReadOnlyList<OrderManagement.Application.DTOs.ProductDto>>(
                "products_all", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var handler = new GetAllProductsQueryHandler(
            _productRepoMock.Object, _mapper, _cacheMock.Object);

        // Act
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Name.Should().Be("Cached");

        // Репозиторий не должен вызываться
        _productRepoMock.Verify(r => r.ListAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_ReadsFromDbAndPopulatesCache()
    {
        // Arrange
        _cacheMock
            .Setup(c => c.GetAsync<IReadOnlyList<OrderManagement.Application.DTOs.ProductDto>>(
                "products_all", It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<OrderManagement.Application.DTOs.ProductDto>?)null);

        var products = new List<Product>
        {
            new("P1", "D1", 10m, 1),
            new("P2", "D2", 20m, 2)
        };
        _productRepoMock
            .Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetAllProductsQueryHandler(
            _productRepoMock.Object, _mapper, _cacheMock.Object);

        // Act
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        _productRepoMock.Verify(r => r.ListAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(
            "products_all",
            It.IsAny<IReadOnlyList<OrderManagement.Application.DTOs.ProductDto>>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}