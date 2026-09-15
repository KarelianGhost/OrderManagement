using FluentAssertions;
using Moq;
using OrderManagement.Application.Features.Customers.Commands;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.UnitTests.Features.Customers;

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<IRepository<Customer>> _customerRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_ExistingCustomer_ReturnsSuccess()
    {
        // Arrange
        var customer = new Customer("Иван", "Петров", "ivan@example.com", "+7999");
        _customerRepoMock
            .Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var handler = new DeleteCustomerCommandHandler(_customerRepoMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(new DeleteCustomerCommand(customer.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _customerRepoMock.Verify(r => r.DeleteAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCustomer_ReturnsFailureWithNotFoundCode()
    {
        // Arrange
        _customerRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var handler = new DeleteCustomerCommandHandler(_customerRepoMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(new DeleteCustomerCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NotFound");
        _customerRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}