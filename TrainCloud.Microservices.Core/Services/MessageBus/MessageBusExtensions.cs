using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.DependencyInjection;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

/// <summary>
/// Singleton
/// https://cloud.google.com/dotnet/docs/reference/Google.Cloud.PubSub.V1/latest#performance-considerations-and-default-settings
/// </summary>
public static class MessageBusExtensions
{
    public static IServiceCollection AddTrainCloudMessageBusPublisher(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBusPublisherService, MessageBusPublisherService>();

        return services;
    }

    public static IServiceCollection AddTrainCloudMessageBusSubscriber(this IServiceCollection services)
    {
        services.AddHostedService<MessageBusSubscriberService>();

        return services;
    }
}
