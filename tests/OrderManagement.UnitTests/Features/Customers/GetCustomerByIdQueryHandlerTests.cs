using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Customers.Queries;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Customers;

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepoMock = new();
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ExistingCustomer_ReturnsSuccess()
    {
        // Arrange
        var customer = new Customer("Иван", "Петров", "ivan@example.com", "+7999");
        _customerRepoMock
            .Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var handler = new GetCustomerByIdQueryHandler(_customerRepoMock.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetCustomerByIdQuery(customer.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task Handle_NonExistingCustomer_ReturnsFailureWithNotFoundCode()
    {
        // Arrange
        _customerRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var handler = new GetCustomerByIdQueryHandler(_customerRepoMock.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetCustomerByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NotFound");
    }
}