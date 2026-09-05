using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Customers.Commands;

public record DeleteCustomerCommand(Guid Id) : IRequest<Result>;