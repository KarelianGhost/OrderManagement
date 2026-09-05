using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Features.Customers.Commands;

public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber) : IRequest<Result<CustomerDto>>;