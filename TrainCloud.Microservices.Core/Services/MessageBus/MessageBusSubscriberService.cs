using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using static Google.Cloud.PubSub.V1.Subscriber;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public class MessageBusSubscriberService : IHostedService//, IDisposable
{
    private Timer? Timer { get; set; }

    private SubscriberServiceApiClient Subscriber {  get; init; }

    public MessageBusSubscriberService()
    {
        Subscriber = SubscriberServiceApiClient.Create();
    }

    private async void DoWorkAsync(object? state)
    {
        string projectId = "traincloud";
        string topicId = "topicId";
        string subscriptionId = "subscriptionId";

        TopicName topicName = new TopicName(projectId, topicId);

        // Subscribe to the topic.
        SubscriptionName subscriptionName = new SubscriptionName(projectId, subscriptionId);

        // Pull messages from the subscription. This will wait for some time if no new messages have been published yet.
        PullResponse response = await Subscriber.PullAsync(subscriptionName, maxMessages: 10);
        foreach (ReceivedMessage received in response.ReceivedMessages)
        {
            PubsubMessage msg = received.Message;
        }

        // Acknowledge that we've received the messages. If we don't do this within 60 seconds (as specified
        // when we created the subscription) we'll receive the messages again when we next pull.
        await Subscriber.AcknowledgeAsync(subscriptionName, response.ReceivedMessages.Select(m => m.AckId));
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        Timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));


        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        Timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
}
