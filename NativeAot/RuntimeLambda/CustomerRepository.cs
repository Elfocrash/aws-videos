using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace RuntimeLambda;

public class Customer
{
    [JsonPropertyName("pk")] public string Pk { get; set; } = default!;

    [JsonPropertyName("sk")] public string Sk { get; set; } = default!;

    [JsonPropertyName("id")] public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("fullName")] public string FullName { get; set; } = default!;

    [JsonPropertyName("email")] public string Email { get; set; } = default!;
}

public class CustomerRepository
{
    private const string TableName = "customers";
    private readonly IAmazonDynamoDB _dynamoDb;
    
    public CustomerRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<bool> CreateAsync(Customer customer)
    {
        var customerAsJson = JsonSerializer.Serialize(customer, HttpApiJsonSerializerContext.Default.Customer);
        var itemAsDocument = Document.FromJson(customerAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = TableName,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<Customer?> GetAsync(Guid id)
    {
        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(request);
        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize(itemAsDocument.ToJson(), HttpApiJsonSerializerContext.Default.Customer);
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        var customerAsJson = JsonSerializer.Serialize(customer, HttpApiJsonSerializerContext.Default.Customer);
        var itemAsDocument = Document.FromJson(customerAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = TableName,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(deleteItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
