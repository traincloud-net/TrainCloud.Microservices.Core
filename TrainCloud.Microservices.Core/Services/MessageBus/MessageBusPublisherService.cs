﻿using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public class MessageBusPublisherService : AbstractService<MessageBusPublisherService>, IMessageBusPublisherService
{
    protected IWebHostEnvironment WebHostEnvironment { get; init; }

    private PublisherServiceApiClient Publisher { get; init; }
 
    public MessageBusPublisherService(IConfiguration configuration,
                                      ILogger<MessageBusPublisherService> logger,
                                      IWebHostEnvironment webHostEnvironment)
        : base(configuration, logger)
    {
        WebHostEnvironment = webHostEnvironment;
        Publisher = PublisherServiceApiClient.Create();
    }

    public async Task SendMessageAsync<TData>(string topicId, TData data)
    {
        string projectId = "traincloud";
        TopicName topicName = new TopicName(projectId, topicId);

        string dataString = JsonSerializer.Serialize(data);

        PubsubMessage message = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(dataString),
            Attributes = { { "EnvironmentName", WebHostEnvironment.EnvironmentName },
                           { "ApplicationName", WebHostEnvironment.ApplicationName }}
        };

        await Publisher.PublishAsync(topicName, new[] { message });
    }
}