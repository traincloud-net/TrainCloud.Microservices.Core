using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

/// <summary>
/// The MessageBusPublisherService is a generic Service for all Microservices, to send a dataobject (Message) to Google Cloud Pub/Sub
/// </summary>
public sealed class MessageBusPublisherService : AbstractService<MessageBusPublisherService>, IMessageBusPublisherService
{
    private IWebHostEnvironment WebHostEnvironment { get; init; }

    public MessageBusPublisherService(IConfiguration configuration,
                                      ILogger<MessageBusPublisherService> logger,
                                      IWebHostEnvironment webHostEnvironment)
        : base(configuration, logger)
    {
        WebHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Sends a dataobject (Message) to to a Google Cloud Pub/Sub topic
    /// </summary>
    /// <typeparam name="TData">Type of the dataobject (Message) to send</typeparam>
    /// <param name="topicId">Google Cloud Pub/Sub topic id</param>
    /// <param name="data">The dataobject (Message) to send</param>
    /// <returns></returns>
    public async Task SendMessageAsync<TData>(string topicId, TData data)
    {
        PublisherServiceApiClient publisher = await PublisherServiceApiClient.CreateAsync();

        string projectId = "traincloud";
        TopicName topicName = new TopicName(projectId, topicId);

        string dataString = JsonSerializer.Serialize(data);
        string trainCloudRegion = Environment.GetEnvironmentVariable("TRAINCLOUD_REGION") ?? "unknown";
        PubsubMessage message = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(dataString),
            Attributes = { { "ApplicationName", WebHostEnvironment.ApplicationName },
                           { "TrainCloud-Environment", WebHostEnvironment.EnvironmentName },
                           { "TrainCloud-Service", trainCloudRegion } }
        };

        await publisher.PublishAsync(topicName, new[] { message });
    }
}
