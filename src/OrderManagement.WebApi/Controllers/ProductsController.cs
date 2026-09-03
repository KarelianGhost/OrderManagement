using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.WebApi.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public ProductsController(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productRepository.ListAsync();
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _productRepository.AddAsync(product);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id, version = "1" },
            _mapper.Map<ProductDto>(product));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        _mapper.Map(dto, product);
        await _productRepository.UpdateAsync(product);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        await _productRepository.DeleteAsync(product);
        return NoContent();
    }
}