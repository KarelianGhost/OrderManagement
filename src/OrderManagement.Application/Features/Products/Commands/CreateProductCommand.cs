using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Features.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity) : IRequest<Result<ProductDto>>;