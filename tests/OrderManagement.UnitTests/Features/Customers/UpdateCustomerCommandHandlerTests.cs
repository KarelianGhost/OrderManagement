using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Customers.Commands;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Customers;

public class UpdateCustomerCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ExistingCustomer_ReturnsSuccessAndUpdatesFields()
    {
        // Arrange
        var customer = new Customer("Иван", "Петров", "ivan@example.com", "+7999");
        _customerRepoMock
            .Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var command = new UpdateCustomerCommand(
            customer.Id, "Пётр", "Сидоров", "petr@example.com", "+7888");

        var handler = new UpdateCustomerCommandHandler(
            _customerRepoMock.Object, _unitOfWorkMock.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.FirstName.Should().Be("Пётр");
        customer.LastName.Should().Be("Сидоров");
        customer.Email.Should().Be("petr@example.com");
        customer.PhoneNumber.Should().Be("+7888");

        _customerRepoMock.Verify(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCustomer_ReturnsFailureWithNotFoundCode()
    {
        // Arrange
        _customerRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var command = new UpdateCustomerCommand(
            Guid.NewGuid(), "A", "B", "a@b.com", "123");

        var handler = new UpdateCustomerCommandHandler(
            _customerRepoMock.Object, _unitOfWorkMock.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NotFound");
        _customerRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}