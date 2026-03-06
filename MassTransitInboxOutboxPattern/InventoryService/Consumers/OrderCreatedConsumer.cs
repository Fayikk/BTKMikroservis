using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shared;

namespace InventoryService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {

        
    private readonly RedisInboxService _inboxService;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    
    private static readonly Dictionary<string, int> _inventory = new()
    {
        ["Laptop"] = 100,
        ["Mouse"] = 200,
        ["Keyboard"] = 150
    };

    public OrderCreatedConsumer(RedisInboxService inboxService, ILogger<OrderCreatedConsumer> logger)
    {
        _inboxService = inboxService;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        var messageId = message.OrderId; 

        _logger.LogInformation("───────────────────────────────────────────────────");
        _logger.LogInformation("📨 Mesaj alındı: {OrderId} - {ProductName} x{Quantity}",
            message.OrderId, message.ProductName, message.Quantity);

        var isProcessed = await _inboxService.IsMessageProcessedAsync(messageId);
        
        if (isProcessed)
        {
            _logger.LogWarning("🚫 DUPLICATE MES AJ! Bu sipariş daha önce işlendi, atlıyorum.");
            _logger.LogWarning("OrderId: {OrderId}", message.OrderId);
            _logger.LogInformation("───────────────────────────────────────────────────");
            return; 
        }

        try
        {
            _logger.LogInformation("💼 İş mantığı çalışıyor: Stok güncelleniyor...");
            
            if (_inventory.ContainsKey(message.ProductName))
            {
                _inventory[message.ProductName] -= message.Quantity;
                _logger.LogInformation("✅ Stok güncellendi: {ProductName} → Kalan: {Stock}",
                    message.ProductName, _inventory[message.ProductName]);
            }
            else
            {
                _logger.LogWarning("⚠️ Ürün stokta bulunamadı: {ProductName}", message.ProductName);
            }

            await Task.Delay(500);

            await _inboxService.MarkAsProcessedAsync(messageId, nameof(OrderCreatedEvent), message);
            
            _logger.LogInformation("✅ Mesaj başarıyla işlendi ve Inbox'a kaydedildi");
            _logger.LogInformation("───────────────────────────────────────────────────");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Mesaj işlenirken hata oluştu: {OrderId}", message.OrderId);
            throw;
        }
    }


    }
}