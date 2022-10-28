using Amazon;
using Amazon.SQS;
using Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<SqsConsumerService>();
builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.EUWest2));

builder.Services.AddSingleton<MessageDispatcher>();
builder.Services.AddMessageHandlers();

var app = builder.Build();

app.Run();
