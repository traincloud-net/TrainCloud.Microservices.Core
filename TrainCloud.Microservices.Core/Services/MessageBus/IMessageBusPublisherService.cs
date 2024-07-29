namespace TrainCloud.Microservices.Core.Services.MessageBus;

public interface IMessageBusPublisherService
{
    Task SendMessageAsync<TData>(string topicId, TData data);
}
