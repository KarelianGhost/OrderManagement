using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Products.Queries;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Products;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Product>> _productRepoMock = new();
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ExistingProduct_ReturnsSuccess()
    {
        var product = new Product("Test", "Desc", 99m, 5);
        _productRepoMock
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new GetProductByIdQueryHandler(_productRepoMock.Object, _mapper);
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(product.Id);
        result.Value.Name.Should().Be("Test");
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ReturnsNotFound()
    {
        _productRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(_productRepoMock.Object, _mapper);
        var result = await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NotFound");
    }
}