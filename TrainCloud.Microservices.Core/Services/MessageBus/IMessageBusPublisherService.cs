using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public interface IMessageBusPublisherService
{
    Task SendAsync(string topicId);
}
