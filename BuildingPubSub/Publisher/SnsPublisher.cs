using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Contracts;

namespace Publisher;

public class SnsPublisher
{
    private readonly IAmazonSimpleNotificationService _sns;

    public SnsPublisher(IAmazonSimpleNotificationService sns)
    {
        _sns = sns;
    }

    public async Task PublishAsync<TMessage>(string topicArn, TMessage message)
        where TMessage : IMessage
    {
        var request = new PublishRequest
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(message)
        };
        request.MessageAttributes.Add(nameof(IMessage.MessageTypeName),
            new MessageAttributeValue
            {
                DataType = "String",
                StringValue = message.MessageTypeName
            });

        foreach (var attribute in message.ToMessageAttributeDictionary())
        {
            request.MessageAttributes.Add(attribute.Key, attribute.Value);
        }

        await _sns.PublishAsync(request);
    }
}
