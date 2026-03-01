using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingApi.Events;

namespace EventSourcingApi.EventStore
{
  public class InMemoryEventStore : IEventStore
{
    private readonly Dictionary<Guid, List<IDomainEvent>> _store = [];
    private readonly List<EventLogEntry> _log = [];
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task SaveAsync(Guid aggregateId, IEnumerable<IDomainEvent> events)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_store.ContainsKey(aggregateId))
                _store[aggregateId] = [];

            foreach (var e in events)
            {
                _store[aggregateId].Add(e);
                _log.Add(new EventLogEntry(e.GetType().Name, e.Version, e.OccurredAt));
                Console.WriteLine($"[EVENT STORE] v{e.Version} {e.GetType().Name} ? {aggregateId}");
            }
        }
        finally { _lock.Release(); }
    }

    public async Task<List<IDomainEvent>> LoadAsync(Guid aggregateId)
    {
        await _lock.WaitAsync();
        try
        {
            return _store.TryGetValue(aggregateId, out var events)
                ? events.OrderBy(e => e.Version).ToList()
                : [];
        }
        finally { _lock.Release(); }
    }

    public async Task<List<EventLogEntry>> GetLogAsync()
    {
        await _lock.WaitAsync();
        try { return [.. _log]; }
        finally { _lock.Release(); }
    }
}

}