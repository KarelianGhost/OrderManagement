using FluentAssertions;
using OrderManagement.Application.Features.Customers.Commands;

namespace OrderManagement.UnitTests.Features.Customers;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator = new();

    [Theory]
    [InlineData("", "Петров", "a@b.com", "Имя обязательно")]
    [InlineData("Иван", "", "a@b.com", "Фамилия обязательна")]
    [InlineData("Иван", "Петров", "not-an-email", "Некорректный формат email")]
    public async Task Validate_InvalidInput_ReturnsExpectedError(
        string firstName, string lastName, string email, string expectedErrorFragment)
    {
        var command = new CreateCustomerCommand(firstName, lastName, email, "123");
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedErrorFragment));
    }

    [Fact]
    public async Task Validate_ValidInput_Passes()
    {
        var command = new CreateCustomerCommand("Иван", "Петров", "ivan@example.com", "+79161234567");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
}