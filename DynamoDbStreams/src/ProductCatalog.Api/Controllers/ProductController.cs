using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Contracts;
using ProductCatalog.Api.Mappers;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Api.Controllers;

[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("products/{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId)
    {
        var product = await _productService.GetByIdAsync(productId);

        if (product is null)
        {
            return NotFound();
        }

        var productResponse = product.ToProductResponse();
        return Ok(productResponse);
    }

    [HttpPost("products")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest createProductRequest)
    {
        var product = createProductRequest.ToProduct();
        var result = await _productService.CreateAsync(product);
        var productResponse = result.ToProductResponse();
        return CreatedAtAction(nameof(GetById), new { productId = productResponse.Id }, productResponse);
    }

    [HttpPut("products/{productId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid productId,
        [FromBody] UpdateProductRequest updateProductRequest)
    {
        var product = updateProductRequest.ToProduct(productId);
        var result = await _productService.CreateAsync(product);
        var productResponse = result.ToProductResponse();
        return Ok(productResponse);
    }

    [HttpDelete("products/{productId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid productId)
    {
        await _productService.DeleteAsync(productId);
        return Ok();
    }
}
