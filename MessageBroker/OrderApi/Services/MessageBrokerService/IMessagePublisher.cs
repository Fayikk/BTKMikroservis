using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.Services.MessageBrokerService
{
    public interface IMessagePublisher
    {
         void PublishOrderCreated<T>(T message);
        
    }
}