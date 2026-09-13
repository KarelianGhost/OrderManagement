using AutoMapper;
using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.IntegrationEvents;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Features.Customers.Commands;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IEventPublisher _eventPublisher;

    private const string CacheKey = "customers_all";

    public CreateCustomerCommandHandler(
        IRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService,
        IEventPublisher eventPublisher)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = _mapper.Map<Customer>(request);
        await _customerRepository.AddAsync(customer, cancellationToken);

        // Публикация интеграционного события
        await _eventPublisher.PublishAsync(new CustomerCreatedEvent
        {
            CustomerId = customer.Id,
            Email = customer.Email,
            FullName = $"{customer.FirstName} {customer.LastName}".Trim(),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Инвалидация кэша
        await _cacheService.RemoveAsync(CacheKey, cancellationToken);

        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }
}