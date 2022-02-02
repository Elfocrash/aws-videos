using ProductCatalog.Api.Mappers;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        var productDto = await _productRepository.GetByIdAsync(productId);
        return productDto?.ToProduct();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        var productDto = product.ToProductDto();
        await _productRepository.CreateAsync(productDto);
        return product;
    }

    public async Task<Product> UpdateAsync(Product updatedProduct)
    {
        var productDto = updatedProduct.ToProductDto();
        await _productRepository.UpdateAsync(productDto);
        return updatedProduct;
    }

    public async Task<bool> DeleteAsync(Guid productId)
    {
        return await _productRepository.DeleteAsync(productId);
    }
}
