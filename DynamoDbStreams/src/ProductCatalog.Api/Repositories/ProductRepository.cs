using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using ProductCatalog.Api.Data;
using StackExchange.Redis;

namespace ProductCatalog.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;
    private readonly IConnectionMultiplexer _redisMultiplexer;

    public ProductRepository(IAmazonDynamoDB dynamoDb,
        IConnectionMultiplexer redisMultiplexer, string tableName)
    {
        _dynamoDb = dynamoDb;
        _tableName = tableName;
        _redisMultiplexer = redisMultiplexer;
    }

    private string GeneratePk(Guid productId) => $"PRODUCT#{productId.ToString()}";

    public async Task<ProductDto?> GetByIdAsync(Guid productId)
    {
        var database = _redisMultiplexer.GetDatabase();
        var redisProduct = await database.StringGetAsync(productId.ToString());
        if (redisProduct != RedisValue.Null)
        {
            return JsonSerializer.Deserialize<ProductDto>(redisProduct);
        }

        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>()
            {
                { "pk", new AttributeValue { S = GeneratePk(productId) } },
                { "sk", new AttributeValue { S = GeneratePk(productId) } }
            },
            ConsistentRead = true
        };

        var response = await _dynamoDb.GetItemAsync(request);
        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        var itemAsJson = itemAsDocument.ToJson();
        await database.StringSetAsync(productId.ToString(), itemAsJson);
        return JsonSerializer.Deserialize<ProductDto>(itemAsDocument.ToJson());
    }

    public async Task<ProductDto> CreateAsync(ProductDto product)
    {
        product.LastUpdatedUtc = DateTime.UtcNow;
        var productAsJson = JsonSerializer.Serialize(product);
        var itemAsDocument = Document.FromJson(productAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = itemAsAttributes
        };

        await _dynamoDb.PutItemAsync(createItemRequest);
        return product;
    }

    public async Task<ProductDto> UpdateAsync(ProductDto updatedProduct)
    {
        //Create and Update is the same method in DynamoDB
        //and since we don't have any difference in the logic
        //I am using then Create method here too
        return await CreateAsync(updatedProduct);
    }

    public async Task<bool> DeleteAsync(Guid productId)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>()
            {
                { "pk", new AttributeValue { S = GeneratePk(productId) } },
                { "sk", new AttributeValue { S = GeneratePk(productId) } }
            }
        };

        await _dynamoDb.DeleteItemAsync(deleteItemRequest);
        return true;
    }
}
