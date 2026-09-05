using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity) : IRequest<Result>;