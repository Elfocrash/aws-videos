using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private readonly ILogger<QueueConsumerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Dictionary<string, Type> _messageTypes;

    public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings, ILogger<QueueConsumerService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _sqs = sqs;
        _queueSettings = queueSettings;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _messageTypes = typeof(Program).Assembly.DefinedTypes
            .Where(x =>
                typeof(ISqsMessage).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .ToDictionary(type => type.Name, type => type.AsType());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, stoppingToken);
        
        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            AttributeNames = new List<string>{ "All" },
            MessageAttributeNames = new List<string>{ "All" },
            MaxNumberOfMessages = 10
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;
                //var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                var type = _messageTypes.GetValueOrDefault(messageType);

                if (type is null)
                {
                    _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                    continue;
                }

                var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;
                
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Message failed during processing");
                    continue;
                }
                
                await _sqs.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}
