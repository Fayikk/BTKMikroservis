using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingApi.Domain.Aggregates;
using EventSourcingApi.EventStore;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   public class OrdersController : ControllerBase
{
    private readonly IEventStore _store;
    public OrdersController(IEventStore store) => _store = store;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequest req)
    {
        var id    = Guid.NewGuid();
        var order = OrderAggregate.Create(id, req.MusteriAdi);
        await _store.SaveAsync(id, order.NewEvents);
        return Ok(new { SiparisId = id });
    }

    [HttpPost("{id}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] AddItemRequest req)
    {
        var order = OrderAggregate.Rehydrate(await _store.LoadAsync(id));
        order.UrunEkle(req.UrunAdi, req.Fiyat);
        await _store.SaveAsync(id, order.NewEvents);
        return Ok();
    }

    [HttpPost("{id}/ship")]
    public async Task<IActionResult> Ship(Guid id, [FromBody] ShipRequest req)
    {
        var order = OrderAggregate.Rehydrate(await _store.LoadAsync(id));
        order.KargoGonder(req.TakipNo);
        await _store.SaveAsync(id, order.NewEvents);
        return Ok(new { TakipNo = req.TakipNo });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var events = await _store.LoadAsync(id);
        if (!events.Any()) return NotFound();

        var order = OrderAggregate.Rehydrate(events);
        return Ok(new
        {
            order.Id,
            order.MusteriAdi,
            order.Kargolandi,
            order.TakipNo,
            order.Version,
            Urunler = order.Urunler,
            Toplam  = order.Urunler.Sum(u => u.Fiyat)
        });
    }

    [HttpGet("event-log")]
    public async Task<IActionResult> EventLog()
    {
        return Ok(await _store.GetLogAsync());
    }
}


}

public record CreateRequest(string MusteriAdi);
public record AddItemRequest(string UrunAdi, decimal Fiyat);
public record ShipRequest(string TakipNo);
