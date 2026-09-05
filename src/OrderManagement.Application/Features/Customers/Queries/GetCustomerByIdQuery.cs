using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Features.Customers.Queries;

public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;