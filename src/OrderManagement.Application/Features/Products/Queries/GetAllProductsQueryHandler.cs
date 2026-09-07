using AutoMapper;
using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Features.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    private const string CacheKey = "products_all";

    public GetAllProductsQueryHandler(
        IRepository<Product> productRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // Пытаемся получить из кэша
        var cached = await _cacheService.GetAsync<IReadOnlyList<ProductDto>>(CacheKey, cancellationToken);
        if (cached is not null)
            return Result<IReadOnlyList<ProductDto>>.Success(cached);

        // Если нет в кэше – читаем из БД
        var products = await _productRepository.ListAsync(cancellationToken);
        var dtos = _mapper.Map<IReadOnlyList<ProductDto>>(products);

        // Сохраняем в кэш на 5 минут
        await _cacheService.SetAsync(CacheKey, dtos, TimeSpan.FromMinutes(5), cancellationToken);

        return Result<IReadOnlyList<ProductDto>>.Success(dtos);
    }
}