using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using static Google.Cloud.PubSub.V1.Subscriber;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public class MessageBusSubscriberService : IHostedService//, IDisposable
{
    protected ILogger<MessageBusSubscriberService> Logger { get; init; }

    protected IWebHostEnvironment WebHostEnvironment { get; init; }
    
    protected string SubscriptionId { get; init; }

    private SubscriberServiceApiClient Subscriber {  get; init; }

    private bool IsRunning { get; set; } = true;

    public MessageBusSubscriberService(ILogger<MessageBusSubscriberService> logger, IWebHostEnvironment webHostEnvironment, string subscriptionId)
    {
        Logger = logger;
        WebHostEnvironment = webHostEnvironment;
        SubscriptionId = subscriptionId;
        Subscriber = SubscriberServiceApiClient.Create();
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
       while(IsRunning)
        {
            string projectId = "traincloud";

            // Subscribe to the topic.
            SubscriptionName subscriptionName = new SubscriptionName(projectId, SubscriptionId);

            // Pull messages from the subscription. This will wait for some time if no new messages have been published yet.
            PullResponse response = await Subscriber.PullAsync(subscriptionName, maxMessages: 10);
            foreach (ReceivedMessage received in response.ReceivedMessages)
            {
                PubsubMessage msg = received.Message;


                Logger.LogWarning(msg.Data.ToString());

                Logger.LogError(msg.Data.ToString());

                Logger.LogCritical(msg.Data.ToString());
            }

            // Acknowledge that we've received the messages. If we don't do this within 60 seconds (as specified
            // when we created the subscription) we'll receive the messages again when we next pull.
            await Subscriber.AcknowledgeAsync(subscriptionName, response.ReceivedMessages.Select(m => m.AckId));
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        IsRunning = false;

        return Task.CompletedTask;
    }
}
