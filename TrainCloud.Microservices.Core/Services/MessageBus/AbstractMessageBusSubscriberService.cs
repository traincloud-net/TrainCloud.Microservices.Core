﻿using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public abstract class AbstractMessageBusSubscriberService<TMessage> : AbstractService<AbstractMessageBusSubscriberService<TMessage>>, IHostedService
{
    private string SubscriptionId { get; init; }

    private SubscriberServiceApiClient Subscriber { get; init; }

    private bool IsRunning { get; set; } = true;

    public AbstractMessageBusSubscriberService(IConfiguration configuration,
                                               ILogger<AbstractMessageBusSubscriberService<TMessage>> logger,
                                               string subscriptionId)
        : base(configuration, logger)
    {
        SubscriptionId = subscriptionId;
        Subscriber = SubscriberServiceApiClient.Create();
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
            SubscriptionName subscriptionName = new SubscriptionName("traincloud", SubscriptionId);

            // Pull messages from the subscription. This will wait for some time if no new messages have been published yet.
            PullResponse response = await Subscriber.PullAsync(subscriptionName, maxMessages: 10);
            foreach (ReceivedMessage received in response.ReceivedMessages)
            {
                PubsubMessage msg = received.Message;

                string messageString = msg.Data.ToStringUtf8();

                TMessage message = JsonSerializer.Deserialize<TMessage>(messageString)!;

                await OnMessageAsync(message);
            }

            // Acknowledge that we've received the messages. If we don't do this within 60 seconds (as specified
            // when we created the subscription) we'll receive the messages again when we next pull.
            if (response.ReceivedMessages.Count > 0)
            {
                await Subscriber.AcknowledgeAsync(subscriptionName, response.ReceivedMessages.Select(m => m.AckId));
            }
        }
    }

    public virtual async Task OnMessageAsync(TMessage message)
    {
        await Task.Delay(0);
        Logger.LogCritical("BaseClassMessage");
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }
}
