using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using sagaapi.Contracts;
namespace sagaapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

    public OrderController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var correlationId = Guid.NewGuid();

        var orderId = Guid.NewGuid();

        Console.WriteLine($"[API] Yeni sipariş isteği geldi. CorrelationId: {correlationId}, OrderId: {orderId}");

        await _publishEndpoint.Publish(new OrderSubmittedEvent { CorrelationId = correlationId, OrderId = orderId });

        return Ok(new
        {
            Message = "Sipariş işleme alındı. Arka planda Saga süreci (Ödeme -> Stok) çalışacak.",
            CorrelationId = correlationId,
            OrderId = orderId
        });
    }
    }
}