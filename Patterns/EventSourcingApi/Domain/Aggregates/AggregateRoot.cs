using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingApi.Events;

namespace EventSourcingApi.Domain.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IDomainEvent> _newEvents = []; 
        public Guid Id      { get; protected set; }
    public int  Version { get; protected set; }
    
    public IReadOnlyList<IDomainEvent> NewEvents => _newEvents;
         protected void RaiseEvent(IDomainEvent @event)
    {
        Apply(@event);
        _newEvents.Add(@event);
    }
    public void LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        foreach (var e in history)
        {
            Apply(e);
            Version = e.Version;
        }
    }
    public void ClearNewEvents() => _newEvents.Clear();
    protected abstract void Apply(IDomainEvent @event);
    }
}