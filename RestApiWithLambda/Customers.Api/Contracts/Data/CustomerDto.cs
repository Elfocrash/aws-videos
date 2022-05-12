using System.Text.Json.Serialization;

namespace Customers.Api.Contracts.Data;

public class CustomerDto
{
    [JsonPropertyName("pk")]
    public string Pk => Id;

    [JsonPropertyName("sk")]
    public string Sk => Id;

    public string Id { get; init; } = default!;

    public string Username { get; init; } = default!;

    public string FullName { get; init; } = default!;

    public string Email { get; init; } = default!;

    public DateTime DateOfBirth { get; init; }
}
