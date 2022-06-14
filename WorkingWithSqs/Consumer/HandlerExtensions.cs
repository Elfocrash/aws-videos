using System.Reflection;
using Consumer.Handlers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Consumer;

public static class HandlerExtensions
{
    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        var handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        foreach (var handler in handlers)
        {
            var handlerType = handler.AsType();
            // var serviceDescriptor = new ServiceDescriptor(handlerType, handlerType, ServiceLifetime.Scoped);
            // services.Add(serviceDescriptor);
            services.AddScoped(handlerType);
        }

        return services;
    }
}
