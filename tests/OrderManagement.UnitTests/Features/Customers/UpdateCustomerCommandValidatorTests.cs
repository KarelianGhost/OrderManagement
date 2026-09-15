using FluentAssertions;
using OrderManagement.Application.Features.Customers.Commands;

namespace OrderManagement.UnitTests.Features.Customers;

public class UpdateCustomerCommandValidatorTests
{
    private readonly UpdateCustomerCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(), "Иван", "Петров", "ivan@example.com", "+79161234567");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyId_Fails()
    {
        var command = new UpdateCustomerCommand(
            Guid.Empty, "Иван", "Петров", "ivan@example.com", "123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCustomerCommand.Id));
    }

    [Theory]
    [InlineData("", "Петров", "a@b.com")]
    [InlineData("Иван", "", "a@b.com")]
    [InlineData("Иван", "Петров", "invalid-email")]
    public async Task Validate_InvalidFields_Fails(string first, string last, string email)
    {
        var command = new UpdateCustomerCommand(Guid.NewGuid(), first, last, email, "123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_PhoneNumberTooLong_Fails()
    {
        var longPhone = new string('1', 21);
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(), "Иван", "Петров", "a@b.com", longPhone);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }
}