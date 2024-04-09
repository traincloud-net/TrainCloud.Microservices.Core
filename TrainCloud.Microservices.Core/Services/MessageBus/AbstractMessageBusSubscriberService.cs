using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public abstract class AbstractMessageBusSubscriberService<TMessage> : AbstractService<AbstractMessageBusSubscriberService<TMessage>>, IHostedService
{
    private bool IsRunning { get; set; } = true;
 
    private string ProjectId { get; init; } = "traincloud";

    private string SubscriptionId { get; set; } = string.Empty;

    private SubscriberServiceApiClient Subscriber { get; init; }

    protected AbstractMessageBusSubscriberService(IConfiguration configuration,
                                                  ILogger<AbstractMessageBusSubscriberService<TMessage>> logger)
        : base(configuration, logger)
    {
        Subscriber = SubscriberServiceApiClient.Create();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task newTask = new(async () => await DoWork());

        newTask.Start();
        
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        while (IsRunning)
        {
            // Subscribe to the topic.
            SubscriptionName subscriptionName = new SubscriptionName(ProjectId, SubscriptionId);

            // Pull messages from the subscription. This will wait for some time if no new messages have been published yet.
            PullResponse response = await Subscriber.PullAsync(subscriptionName, maxMessages: 10);
            foreach (ReceivedMessage received in response.ReceivedMessages)
            {
                PubsubMessage msg = received.Message;

                string messageString = msg.Data.ToStringUtf8();

                TMessage message = JsonSerializer.Deserialize<TMessage>(messageString)!;

                OnMessage(message);
            }

            // Acknowledge that we've received the messages. If we don't do this within 60 seconds (as specified
            // when we created the subscription) we'll receive the messages again when we next pull.
            if (response.ReceivedMessages.Count > 0)
            {
                await Subscriber.AcknowledgeAsync(subscriptionName, response.ReceivedMessages.Select(m => m.AckId));
            }
        }
    }

    public virtual void OnMessage(TMessage message)
    {
        Logger.LogInformation("Message");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }
}
;