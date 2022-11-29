using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;

namespace NativeAotLambda;

public class Function
{
    private static CustomerRepository _customerRepository;
    
    private static async Task Main()
    {
        _customerRepository = new(new AmazonDynamoDBClient());
        Func<APIGatewayHttpApiV2ProxyRequest, ILambdaContext, Task<APIGatewayHttpApiV2ProxyResponse>> handler = FunctionHandler;
        await LambdaBootstrapBuilder.Create(handler,
                new SourceGeneratorLambdaJsonSerializer<HttpApiJsonSerializerContext>())
            .Build()
            .RunAsync();
    }
    
    public static async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest apigProxyEvent, ILambdaContext context)
    {
        var customer = await _customerRepository.GetAsync(Guid.Parse("f702dc75-1be9-4c15-98ba-73c5956d09af"));

        var text =  JsonSerializer.Serialize(customer, HttpApiJsonSerializerContext.Default.Customer!);
        
        return new APIGatewayHttpApiV2ProxyResponse()
        {
            StatusCode = 200,
            Body = text,
            Headers = Headers
        };
        /*
        // API Handling logic here
        return new APIGatewayHttpApiV2ProxyResponse()
        {
            StatusCode = 200,
            Body = "OK"
        };*/
    }
    
    private static readonly Dictionary<string, string> Headers = new()
    {
        { "content-type", "application/json" }
    };
}

[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(Customer))]
public partial class HttpApiJsonSerializerContext : JsonSerializerContext
{
}
