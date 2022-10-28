using System.Text.Json.Serialization;

namespace Publisher.Messages;

public class CustomerDeleted : IMessage
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonIgnore] 
    public string MessageTypeName => nameof(CustomerDeleted);
}
