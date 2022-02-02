using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Mappers;

public static class DomainToDtoMapper
{
    public static ProductDto ToProductDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id.ToString(),
            Currency = product.Currency,
            Description = product.Description,
            Name = product.Name,
            Price = product.Price
        };
    }

    public static Product ToProduct(this ProductDto productDto)
    {
        return new Product
        {
            Id = Guid.Parse(productDto.Id),
            Currency = productDto.Currency,
            Description = productDto.Description,
            Name = productDto.Name,
            Price = productDto.Price
        };
    }
}
