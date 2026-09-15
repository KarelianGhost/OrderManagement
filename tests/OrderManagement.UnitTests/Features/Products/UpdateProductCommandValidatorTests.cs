using FluentAssertions;
using OrderManagement.Application.Features.Products.Commands;

namespace OrderManagement.UnitTests.Features.Products;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Test", "Desc", 99.99m, 10);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyId_Fails()
    {
        var command = new UpdateProductCommand(Guid.Empty, "Test", "Desc", 10m, 1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateProductCommand.Id));
    }

    [Fact]
    public async Task Validate_NegativePrice_Fails()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Test", "Desc", -5m, 1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_NegativeStock_Fails()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Test", "Desc", 10m, -1);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }
}