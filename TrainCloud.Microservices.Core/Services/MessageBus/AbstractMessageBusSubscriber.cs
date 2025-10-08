using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public abstract class AbstractMessageBusSubscriber<TMessage> : AbstractService<AbstractMessageBusSubscriber<TMessage>>, IHostedService
{
    protected IConnection? Connection { get; private set; }

    protected IChannel? Channel { get; private set; }

    protected bool IsRunning { get; private set; } = true;

    private string QueueName { get; init; }

    protected AbstractMessageBusSubscriber(IConfiguration configuration, 
                                           ILogger<AbstractMessageBusSubscriber<TMessage>> logger,
                                           string queueName) 
        : base(configuration, logger)
    {
        QueueName = queueName;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string messageBusHostName = Environment.GetEnvironmentVariable("MESSAGEBUS_HOSTNAME")!;
        string messageBusUserName = Environment.GetEnvironmentVariable("MESSAGEBUS_USERNAME")!;
        string messageBusPassword = Environment.GetEnvironmentVariable("MESSAGEBUS_PASSWORD")!;
        try
        {
            ConnectionFactory factory = new()
            {
                HostName = messageBusHostName,
                Port = 5672,
                UserName = messageBusUserName,
                Password = messageBusPassword
            };

            Connection = await factory.CreateConnectionAsync(cancellationToken);
            Channel = await Connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await Channel.QueueDeclareAsync(queue: QueueName,
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null,
                                            cancellationToken: cancellationToken);

            await Channel.BasicQosAsync(prefetchSize: 0,
                                        prefetchCount: 1,
                                        global: false,
                                        cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(Channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                if (!IsRunning)
                {
                    return;
                }

                try
                {
                    byte[] body = ea.Body.ToArray();
                    string messageJson = System.Text.Encoding.UTF8.GetString(body);

                    TMessage? message = JsonSerializer.Deserialize<TMessage>(messageJson);
                    if (message is not null)
                    {
                        await OnMessageAsync(message);
                        await Channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        await Channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    await Channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await Channel.BasicConsumeAsync(queue: QueueName,
                                            autoAck: false,
                                            consumer: consumer,
                                            cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, $"Failed to start RabbitMQ subscriber for queue '{QueueName}'");
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
