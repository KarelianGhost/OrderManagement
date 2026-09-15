using FluentAssertions;
using OrderManagement.Application.Features.Products.Commands;

namespace OrderManagement.UnitTests.Features.Products;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var command = new CreateProductCommand("Test", "Desc", 99.99m, 10);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Цена не может быть отрицательной")]        // пустое имя
    [InlineData("  ", "Цена не может быть отрицательной")]      // пробелы
    public async Task Validate_EmptyName_Fails(string name, string _)
    {
        var command = new CreateProductCommand(name, "Desc", 10m, 1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public async Task Validate_NegativePrice_Fails()
    {
        var command = new CreateProductCommand("Test", "Desc", -1m, 1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Price));
    }

    [Fact]
    public async Task Validate_PriceWithMoreThanTwoDecimals_Fails()
    {
        var command = new CreateProductCommand("Test", "Desc", 10.999m, 1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Price));
    }

    [Fact]
    public async Task Validate_NegativeStock_Fails()
    {
        var command = new CreateProductCommand("Test", "Desc", 10m, -1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.StockQuantity));
    }

    [Fact]
    public async Task Validate_NameTooLong_Fails()
    {
        var longName = new string('A', 201);
        var command = new CreateProductCommand(longName, "Desc", 10m, 1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }
}