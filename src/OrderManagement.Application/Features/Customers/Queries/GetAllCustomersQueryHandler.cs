using AutoMapper;
using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Features.Customers.Queries;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<IReadOnlyList<CustomerDto>>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper _mapper;

    public GetAllCustomersQueryHandler(IRepository<Customer> customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.ListAsync(cancellationToken);
        return Result<IReadOnlyList<CustomerDto>>.Success(_mapper.Map<IReadOnlyList<CustomerDto>>(customers));
    }
}