using Amazon;
using Amazon.DynamoDBv2;
using ProductCatalog.Api.Repositories;
using ProductCatalog.Api.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
    new AmazonDynamoDBClient(
        RegionEndpoint.GetBySystemName(config.GetValue<string>("DynamoDb:Region"))));
builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis:ConnectionString")));
builder.Services.AddSingleton<IProductRepository>(x =>
    new ProductRepository(
        x.GetRequiredService<IAmazonDynamoDB>(),
        x.GetRequiredService<IConnectionMultiplexer>(), config.GetValue<string>("DynamoDb:TableName")));
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
