namespace TrainCloud.Microservices.Core.Services.MessageBus;

/// <summary>
/// The MessageBusPublisherService is a generic Service for all Microservices, to send a dataobject (Message) to Google Cloud Pub/Sub
/// </summary>
public interface IMessageBusPublisher
{
    Task SendMessageAsync<TData>(string queueName, TData data);
}
