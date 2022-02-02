using ProductCatalog.Api.Contracts;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Mappers;

public static class ContractToDomainMapper
{
    public static Product ToProduct(this CreateProductRequest updateProductRequest)
    {
        return new Product
        {
            Currency = updateProductRequest.Currency,
            Description = updateProductRequest.Description,
            Name = updateProductRequest.Name,
            Price = updateProductRequest.Price
        };
    }

    public static Product ToProduct(this UpdateProductRequest updateProductRequest, Guid productId)
    {
        return new Product
        {
            Id = productId,
            Currency = updateProductRequest.Currency,
            Description = updateProductRequest.Description,
            Name = updateProductRequest.Name,
            Price = updateProductRequest.Price
        };
    }

    public static ProductResponse ToProductResponse(this Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Currency = product.Currency,
            Description = product.Description,
            Name = product.Name,
            Price = product.Price
        };
    }
}
