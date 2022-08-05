using System.Text.Json;
using Amazon.SimpleNotificationService.Model;
using Contracts;

namespace Publisher;

public static class MessageExtensions
{
    public static Dictionary<string, MessageAttributeValue> ToMessageAttributeDictionary<T>(
        this T item) where T : IMessage
    {
        var document = JsonSerializer.SerializeToDocument(item);
        var objectEnumerator = document.RootElement.EnumerateObject();
        var dictionary = new Dictionary<string, MessageAttributeValue>();
        foreach (var jsonProperty in objectEnumerator)
        {
            dictionary.Add(jsonProperty.Name, new MessageAttributeValue
            {
                StringValue = jsonProperty.Value.ToString(),
                DataType = jsonProperty.Value.ValueKind switch
                {
                    JsonValueKind.Number => "Number",
                    JsonValueKind.String => "String",
                    _ => throw new NotSupportedException("This is a demo, sorry")
                }
            });
        }
        return dictionary;
    }
}
