using System.Text.Json.Serialization;

namespace Contracts;

public class CustomerCreated : IMessage
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = default!;

    [JsonPropertyName("lifetimeSpent")]
    public decimal LifetimeSpent { get; set; }
    
    [JsonIgnore] 
    public string MessageTypeName => nameof(CustomerCreated);
}
