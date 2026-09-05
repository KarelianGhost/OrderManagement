using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Features.Products.Queries;

public record GetAllProductsQuery : IRequest<Result<IReadOnlyList<ProductDto>>>;