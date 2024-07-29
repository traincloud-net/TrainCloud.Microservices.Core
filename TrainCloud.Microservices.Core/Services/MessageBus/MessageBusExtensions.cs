using Microsoft.Extensions.DependencyInjection;

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
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTrainCloudMessageBusPublisher(this IServiceCollection services)
    {
        services.AddScoped<IMessageBusPublisherService, MessageBusPublisherService>();

        return services;
    }
}
