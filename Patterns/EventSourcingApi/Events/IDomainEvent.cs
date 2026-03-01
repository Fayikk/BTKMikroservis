using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcingApi.Events
{
    public interface IDomainEvent
    {
        Guid  AggregateId { get; }  // Hangi sipariş?
        int  Version     { get; }  // Kaçıncı event?
        DateTime OccurredAt  { get; }  // Ne zaman?
    }
}