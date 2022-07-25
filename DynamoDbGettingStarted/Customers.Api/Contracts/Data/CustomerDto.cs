using System.Text.Json.Serialization;

namespace Customers.Api.Contracts.Data;

public class CustomerDto
{
    [JsonPropertyName("pk")]
    public string Pk => Id;

    [JsonPropertyName("sk")]
    public string Sk => Pk;
    
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("gitHubUsername")]
    public string GitHubUsername { get; init; } = default!;

    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = default!;

    [JsonPropertyName("email")]
    public string Email { get; init; } = default!;

    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; init; }
}
