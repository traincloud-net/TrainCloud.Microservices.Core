﻿using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

/// <summary>
/// Hosted Service for the Microservice to reveive Pub/Sub subscription messages
/// Does the generic processing, so that the inheriting implementation just has to process the expected Message
/// </summary>
/// <typeparam name="TMessage">The type of the Message to receive for the requested subscription</typeparam>
public abstract class AbstractMessageBusSubscriberService<TMessage> : AbstractService<AbstractMessageBusSubscriberService<TMessage>>, IHostedService
{
    protected IServiceScopeFactory ServiceScopeFactory { get; init; }

    protected string SubscriptionId { get; init; }

    protected bool IsRunning { get; private set; } = true;

    protected bool IsSingleRegionService { get; private set; } = true;

    protected AbstractMessageBusSubscriberService(IConfiguration configuration,
                                                  ILogger<AbstractMessageBusSubscriberService<TMessage>> logger,
                                                  IServiceScopeFactory serviceScopeFactory,
                                                  string subscriptionId,
                                                  bool isSingleRegionService)
        : base(configuration, logger)
    {
        ServiceScopeFactory = serviceScopeFactory;
        SubscriptionId = subscriptionId;
        IsSingleRegionService = isSingleRegionService;
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
            try
            {
                string trainCloudRegion = Environment.GetEnvironmentVariable("TRAINCLOUD_REGION") ?? "unknown";

                // Subscribe to the topic.
                SubscriberServiceApiClient subscriberClient = await SubscriberServiceApiClient.CreateAsync();

                SubscriptionName subscriptionName = new SubscriptionName("traincloud", SubscriptionId);

                // Pull messages from the subscription. This will wait for some time if no new messages have been published yet.
                PullResponse? response = default;

                response = await subscriberClient.PullAsync(subscriptionName, maxMessages: 10);

                foreach (ReceivedMessage received in response.ReceivedMessages)
                {
                    received.Message.Attributes.TryGetValue("TrainCloud-Service", out string senderTrainCloudRegion);
                    MemoryStream msMessage = new(received.Message.Data.ToByteArray());
                    TMessage? message = await JsonSerializer.DeserializeAsync<TMessage>(msMessage);

                    //Process the message only if the sender is in the same region or if the implementation is in running in a single instance
                    // To prevent all redundant servers are processing the same message
                    if(IsSingleRegionService ||
                       trainCloudRegion.ToLower() == senderTrainCloudRegion.ToLower())
                    {
                        // Raise the OnMessage Event in the implementation to process the message
                        await OnMessageAsync(message!);
                    }
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
                Logger.LogError(ex.Message);
            }
        }
    }

    public virtual async Task OnMessageAsync(TMessage message)
    {
        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }
}
