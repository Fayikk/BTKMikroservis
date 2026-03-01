using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingApi.Events;

namespace EventSourcingApi.Domain.Aggregates
{
    public class OrderAggregate : AggregateRoot
    {
         public string  MusteriAdi { get; private set; } = "";
        public bool    Kargolandi  { get; private set; }
        public string? TakipNo    { get; private set; }
        public List<(string UrunAdi, decimal Fiyat)> Urunler { get; } = [];

        public OrderAggregate() {}

         public static OrderAggregate Create(Guid id, string musteriAdi)
    {
        var order = new OrderAggregate();
        order.RaiseEvent(new OrderCreatedEvent
        {
            AggregateId = id,
            Version     = 1,
            MusteriAdi  = musteriAdi
        });
        return order;
    }

public void UrunEkle(string urunAdi, decimal fiyat)
    {
        if (Kargolandi) throw new Exception("Kargoya verilmis siparise �r�n eklenemez.");

        RaiseEvent(new ItemAddedEvent
        {
            AggregateId = Id,
            Version     = Version + 1,
            UrunAdi     = urunAdi,
            Fiyat       = fiyat
        });
    }

    public void KargoGonder(string takipNo)
    {
        if (Kargolandi)     throw new Exception("Zaten kargoya verildi.");
        if (!Urunler.Any()) throw new Exception("�r�n olmadan kargo g�nderilemez.");

        RaiseEvent(new OrderShippedEvent
        {
            AggregateId = Id,
            Version     = Version + 1,
            TakipNo     = takipNo
        });
    }

    protected override void Apply(IDomainEvent @event)
    {
        switch (@event)
        {
            case OrderCreatedEvent e:
                Id         = e.AggregateId;
                MusteriAdi = e.MusteriAdi;
                Version    = e.Version;
                break;

            case ItemAddedEvent e:
                Urunler.Add((e.UrunAdi, e.Fiyat));
                Version = e.Version;
                break;

            case OrderShippedEvent e:
                Kargolandi = true;
                TakipNo    = e.TakipNo;
                Version    = e.Version;
                break;
        }
    }

    public static OrderAggregate Rehydrate(IEnumerable<IDomainEvent> history)
    {
        var order = new OrderAggregate();
        order.LoadFromHistory(history);
        return order;
    }
    }
}