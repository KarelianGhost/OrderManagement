using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Customers.Commands;

public record UpdateCustomerCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber) : IRequest<Result>;