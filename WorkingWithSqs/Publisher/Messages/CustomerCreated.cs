using System.Text.Json.Serialization;

namespace Publisher.Messages;

public class CustomerCreated : IMessage
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = default!;

    [JsonIgnore] 
    public string MessageTypeName => nameof(CustomerCreated);
}
