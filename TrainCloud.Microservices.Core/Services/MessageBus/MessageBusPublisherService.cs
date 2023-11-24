using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public class MessageBusPublisherService : AbstractService<MessageBusPublisherService>, IMessageBusPublisherService
{
     private PublisherServiceApiClient Publisher { get; init; }

    public MessageBusPublisherService(IConfiguration configuration,
                                      ILogger<MessageBusPublisherService> logger)
        : base(configuration, logger)
    {
        Publisher = PublisherServiceApiClient.Create();
    }

    public async Task SendAsync(string topicId)
    {
        string projectId = "traincloud";

        TopicName topicName = new TopicName(projectId, topicId);

        // Publish a message to the topic.
        PubsubMessage message = new PubsubMessage
        {
            // The data is any arbitrary ByteString. Here, we're using text.
            Data = ByteString.CopyFromUtf8("Hello, Pubsub"),

            // The attributes provide metadata in a string-to-string dictionary.
            Attributes = { { "UserId", "user.Id.ToString()" } }
        };

        await Publisher.PublishAsync(topicName, new[] { message });
    }
}
