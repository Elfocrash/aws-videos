using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Services;

public interface IProductService
{
    Task<Product?> GetByIdAsync(Guid productId);

    Task<Product> CreateAsync(Product product);

    Task<Product> UpdateAsync(Product updatedProduct);

    Task<bool> DeleteAsync(Guid productId);
}
