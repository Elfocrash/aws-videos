using ProductCatalog.Api.Data;

namespace ProductCatalog.Api.Repositories;

public interface IProductRepository
{
    Task<ProductDto?> GetByIdAsync(Guid productId);

    Task<ProductDto> CreateAsync(ProductDto product);

    Task<ProductDto> UpdateAsync(ProductDto updatedProduct);

    Task<bool> DeleteAsync(Guid productId);
}
