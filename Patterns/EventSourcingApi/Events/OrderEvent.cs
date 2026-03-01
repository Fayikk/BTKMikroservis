using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcingApi.Events
{
  public abstract record EventBase : IDomainEvent
    {
        public Guid     AggregateId { get; init; }
        public int      Version     { get; init; }
        public DateTime OccurredAt  { get; } = DateTime.UtcNow;
    }
    public record OrderCreatedEvent : EventBase
    {
        public string MusteriAdi { get; init; } = "";
    }
    public record ItemAddedEvent : EventBase
    {
        public string  UrunAdi { get; init; } = "";
        public decimal Fiyat   { get; init; }
    }
    public record OrderShippedEvent : EventBase
    {
        public string TakipNo { get; init; } = "";
    }
}