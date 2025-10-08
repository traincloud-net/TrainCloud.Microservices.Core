using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public sealed class MessageBusPublisher : AbstractService<MessageBusPublisher>, IMessageBusPublisher
{
    public MessageBusPublisher(IConfiguration configuration, 
                               ILogger<MessageBusPublisher> logger) 
        : base(configuration, logger)
    {

    }

    public async Task SendMessageAsync<TData>(string queueName, TData data)
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

            using IConnection connection = await factory.CreateConnectionAsync();
            using IChannel channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName,
                durable: true,  
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            string dataJson = JsonSerializer.Serialize(data);
            var dataBytes = Encoding.UTF8.GetBytes(dataJson);

            await channel.BasicPublishAsync(exchange: "",           // Default exchange
                                            routingKey: queueName,    // Queue name as routing key
                                            body: dataBytes,
                                            mandatory: false);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex.Message, ex);
        }
    }
}
