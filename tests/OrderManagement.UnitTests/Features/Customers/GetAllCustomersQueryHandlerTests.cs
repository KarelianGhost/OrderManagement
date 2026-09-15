using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Customers.Queries;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Customers;

public class GetAllCustomersQueryHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepoMock = new();
    private readonly IMapper _mapper;

    public GetAllCustomersQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ReturnsListOfCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new("Иван", "Петров", "i@example.com", "1"),
            new("Пётр", "Сидоров", "p@example.com", "2")
        };
        _customerRepoMock
            .Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var handler = new GetAllCustomersQueryHandler(_customerRepoMock.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetAllCustomersQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_NoCustomers_ReturnsEmptyList()
    {
        // Arrange
        _customerRepoMock
            .Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        var handler = new GetAllCustomersQueryHandler(_customerRepoMock.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetAllCustomersQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}