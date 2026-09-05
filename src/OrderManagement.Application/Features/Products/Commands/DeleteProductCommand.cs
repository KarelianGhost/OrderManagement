using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;