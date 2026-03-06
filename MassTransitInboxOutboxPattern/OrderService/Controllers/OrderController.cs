using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Outbox;
using Shared;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
    private readonly RedisOutboxService _outboxService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IPublishEndpoint publishEndpoint,
        RedisOutboxService outboxService,
        ILogger<OrderController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _outboxService = outboxService;
        _logger = logger;
    }

 [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderId = Guid.NewGuid();

        var orderEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            TotalPrice = request.Quantity * request.UnitPrice,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Sipariş oluşturuluyor: {OrderId} - {ProductName} x{Quantity}",
            orderId, request.ProductName, request.Quantity);

        try
        {
            await _outboxService.SavePendingMessageAsync(orderId, orderEvent);

            await _publishEndpoint.Publish(orderEvent);

            await _outboxService.MarkAsSentAsync(orderId);

            _logger.LogInformation("✅ OrderCreatedEvent yayınlandı (Outbox Pattern): {OrderId}", orderId);

            return Ok(new
            {
                success = true,
                orderId,
                message = "Sipariş oluşturuldu ve mesaj gönderildi (Outbox Pattern ✅)",
                data = orderEvent
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Mesaj yayınlama hatası: {OrderId}", orderId);

            return StatusCode(500, new
            {
                success = false,
                orderId,
                message = "Hata oluştu ama mesaj outbox'ta saklandı, retry edilecek",
                error = ex.Message
            });
        }
    }


[HttpPost("duplicate-test/{count}")]
    public async Task<IActionResult> PublishDuplicates(int count, [FromBody] CreateOrderRequest request)
    {
        var orderId = Guid.NewGuid();

        var orderEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            TotalPrice = request.Quantity * request.UnitPrice,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogWarning("DUPLICATE TEST: Aynı mesajı {Count} kez yayınlıyorum (Inbox Pattern testi)!", count);

        for (int i = 0; i < count; i++)
        {
            await _publishEndpoint.Publish(orderEvent);
            _logger.LogInformation("Mesaj {Index}/{Count} gönderildi", i + 1, count);
            await Task.Delay(100);
        }

        return Ok(new
        {
            success = true,
            orderId,
            duplicateCount = count,
            message = $"Aynı mesaj {count} kez gönderildi (Inbox Pattern test)",
            note = "InventoryService bu mesajlardan sadece BİRİNİ işlemelidir!"
        });
    }
  [HttpGet("outbox/stats")]
    public async Task<IActionResult> GetOutboxStats()
    {
        var stats = await _outboxService.GetStatsAsync();

        return Ok(new
        {
            success = true,
            stats,
            message = "Outbox Pattern istatistikleri"
        });
    }

    }
}

public record CreateOrderRequest(string ProductName,int Quantity,decimal UnitPrice);