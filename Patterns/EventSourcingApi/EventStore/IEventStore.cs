using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingApi.Events;

namespace EventSourcingApi.EventStore
{

    public record EventLogEntry(string EventType, int Version, DateTime OccurredAt);


  public interface IEventStore
    {
        Task SaveAsync(Guid aggregateId, IEnumerable<IDomainEvent> events);
        Task<List<IDomainEvent>> LoadAsync(Guid aggregateId);
        Task<List<EventLogEntry>> GetLogAsync();
    }
}