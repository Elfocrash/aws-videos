namespace Customers.Consumer.Messages;

public class CustomerDeleted : ISqsMessage
{
    public required Guid Id { get; init; }
}
