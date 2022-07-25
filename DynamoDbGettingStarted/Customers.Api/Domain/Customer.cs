using Customers.Api.Domain.Common;

namespace Customers.Api.Domain;

public class Customer
{
    public CustomerId Id { get; init; } = CustomerId.From(Guid.NewGuid());

    public GitHubUsername GitHubUsername { get; init; } = default!;

    public FullName FullName { get; init; } = default!;

    public Email Email { get; init; } = default!;

    public DateOfBirth DateOfBirth { get; init; } = default!;
}
