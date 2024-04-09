using Google.Cloud.PubSub.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public static class MessageBusExtensions
{
    /// <summary>
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

    public static IServiceCollection AddTrainCloudMessageBusSubscription(this IServiceCollection services, string subscriptionId)
    {
        services.AddHostedService(serviceProvider =>
        {
            IServiceScope scope = serviceProvider.CreateScope();

            IWebHostEnvironment webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            ILogger<MessageBusSubscriberService> logger = scope.ServiceProvider.GetRequiredService<ILogger<MessageBusSubscriberService>>();

            return new MessageBusSubscriberService(logger, webHostEnvironment, subscriptionId);
        });

        return services;
    }
}
