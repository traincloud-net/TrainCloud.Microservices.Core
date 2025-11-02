using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

/// <summary>
/// Extensions for MessageBusPublisherService
/// </summary>
public static class MessageBusExtensions 
{
    /// <summary>
    /// Adds the MessageBusPublisherService to the Microservice DI container.
    /// Singleton
    /// https://cloud.google.com/dotnet/docs/reference/Google.Cloud.PubSub.V1/latest#performance-considerations-and-default-settings
    /// </summary>
    /// <param name="services">ServiceCollection</param>
    /// <param name="lifeTime">Scoped for WebApi (default) / Singleton for Workers</param>
    /// <returns></returns>
    public static IServiceCollection AddTrainCloudMessageBusPublisher(this IServiceCollection services, 
                                                                      ServiceLifetime lifeTime = ServiceLifetime.Scoped)
    {
        Type serviceType = typeof(IMessageBusPublisher);
        Type implementationType = typeof(MessageBusPublisher);

        ServiceDescriptor serviceDescriptor = new(serviceType, implementationType, lifeTime);

        services.Add(serviceDescriptor);

        return services;
    }
}
