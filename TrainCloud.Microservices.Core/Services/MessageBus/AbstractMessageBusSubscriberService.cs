using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public abstract class AbstractMessageBusSubscriberService<TMessage> : AbstractService<AbstractMessageBusSubscriberService<TMessage>>, IHostedService
{
    protected IServiceScopeFactory ServiceScopeFactory { get; init; }

    private string SubscriptionId { get; init; }

    private bool IsRunning { get; set; } = true;

    public AbstractMessageBusSubscriberService(IConfiguration configuration,
                                               ILogger<AbstractMessageBusSubscriberService<TMessage>> logger,
                                               IServiceScopeFactory serviceScopeFactory,
                                               string subscriptionId)
        : base(configuration, logger)
    {
        ServiceScopeFactory = serviceScopeFactory;
        SubscriptionId = subscriptionId;
    }

    public Task StartAsync(CancellationToken stoppingToken)
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
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            SubscriptionName subscriptionName = new SubscriptionName("traincloud", SubscriptionId);

            // Pull messages from the subscription. This will wait for some time if no new messages have been published yet.
            PullResponse? response = default;
            try
            {
                response = await subscriberClient.PullAsync(subscriptionName, maxMessages: 10);

                foreach (ReceivedMessage received in response.ReceivedMessages)
                {
                    MemoryStream msMessage = new(received.Message.Data.ToByteArray());

                    TMessage? message = await JsonSerializer.DeserializeAsync<TMessage>(msMessage);

                    await OnMessageAsync(message!);
                }

                // Acknowledge that we've received the messages. If we don't do this within 60 seconds (as specified
                // when we created the subscription) we'll receive the messages again when we next pull.
                if (response.ReceivedMessages.Count > 0)
                {
                    await subscriberClient.AcknowledgeAsync(subscriptionName, response.ReceivedMessages.Select(m => m.AckId));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
        }
    }

    public virtual async Task OnMessageAsync(TMessage message)
    {
        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }
}
