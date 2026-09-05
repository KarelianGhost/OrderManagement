using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Features.Customers.Queries;

public record GetAllCustomersQuery : IRequest<Result<IReadOnlyList<CustomerDto>>>;