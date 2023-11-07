using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Customers.Api.Contracts.Data;

namespace Customers.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName = "customers";

    public CustomerRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var itemAsDocument = Document.FromJson(customerAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var request = new GetItemRequest
        {
            TableName = _tableName,
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
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var scanRequest = new ScanRequest
        {
            TableName = _tableName
        };
        var response = await _dynamoDb.ScanAsync(scanRequest);
        return response.Items
            .Select(x =>
            {
                var attributeMap = Document.FromAttributeMap(x);
                var json = attributeMap.ToJson();
                return JsonSerializer.Deserialize<CustomerDto>(json);
            })!;
    }

    public async Task<bool> UpdateAsync(CustomerDto customer, DateTime requestStarted)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var itemAsDocument = Document.FromJson(customerAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = itemAsAttributes,
            ConditionExpression = "UpdatedAt < :requestStarted",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":requestStarted", new AttributeValue { S = requestStarted.ToString("O") } }
            }
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(deleteItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    private async Task<string?> LastUpdatedDateForCustomer(string id)
    {
        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id } },
                { "sk", new AttributeValue { S = id } }
            },
            AttributesToGet = new List<string> { nameof(CustomerDto.UpdatedAt) }
        };

        var response = await _dynamoDb.GetItemAsync(request);
        if (response.Item.Count == 0)
        {
            return null;
        }

        return response.Item[nameof(CustomerDto.UpdatedAt)].S;
    }
}
