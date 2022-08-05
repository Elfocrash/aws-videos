using Contracts;

namespace Consumer.Handlers;

public class CustomerUpdatedHandler : IMessageHandler
{
    private readonly ILogger<CustomerUpdatedHandler> _logger;

    public CustomerUpdatedHandler(ILogger<CustomerUpdatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IMessage message)
    {
        var customerUpdated = (CustomerUpdated)message;
        _logger.LogInformation("Customer updated with name: {FullName} " +
                               "and lifetime spent {LifetimeValue}", 
            customerUpdated.FullName, customerUpdated.LifetimeSpent);
        return Task.CompletedTask;
    }

    public static Type MessageType { get; } = typeof(CustomerUpdated);
}
