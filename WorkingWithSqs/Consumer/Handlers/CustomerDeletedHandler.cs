using Consumer.Messages;

namespace Consumer.Handlers;

public class CustomerDeletedHandler : IMessageHandler
{
    private readonly ILogger<CustomerDeletedHandler> _logger;

    public CustomerDeletedHandler(ILogger<CustomerDeletedHandler> logger)
    {
        _logger = logger;
    }
    
    public Task HandleAsync(IMessage message)
    {
        var customerDeleted = (CustomerDeleted)message;
        _logger.LogInformation("Customer deleted with id: {Id}", 
            customerDeleted.Id);
        return Task.CompletedTask;
    }

    public static Type MessageType { get; } = typeof(CustomerDeleted);
}
