using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.DynamoDBEvents;
using StackExchange.Redis;

namespace ProductCatalogCacheSync.Lambda
{
    public class Function
    {
        private static IConnectionMultiplexer _connectionMultiplexer;

        private static readonly string RedisConnectionString =
            Environment.GetEnvironmentVariable("RedisConnectionString");

        private static async Task Main(string[] args)
        {
            _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(RedisConnectionString);
            Func<DynamoDBEvent, ILambdaContext, Task> handler = FunctionHandler;
            await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
                .Build()
                .RunAsync();
        }

        public static async Task FunctionHandler(DynamoDBEvent dynamoDbEvent, ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning handling of dynamodb updates. Document count {dynamoDbEvent.Records.Count}");
            var database = _connectionMultiplexer.GetDatabase();
            foreach (var dynamodbRecord in dynamoDbEvent.Records)
            {
                if (dynamodbRecord.EventName == OperationType.REMOVE)
                {
                    var deletedId = dynamodbRecord.Dynamodb.OldImage["id"].S;
                    await database.KeyDeleteAsync(deletedId);
                    continue;
                }

                var recordAsDocument = Document.FromAttributeMap(dynamodbRecord.Dynamodb.NewImage);
                var json = recordAsDocument.ToJson();
                var id = dynamodbRecord.Dynamodb.NewImage["id"].S;
                await database.StringSetAsync(id, json);
            }

            context.Logger.LogLine($"Completed handling {dynamoDbEvent.Records.Count} documents");
        }
    }
}
