using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using sagaapi.Contracts;

namespace sagaapi.Consumers
{
   public class InventoryConsumer : IConsumer<UpdateInventoryCommand>
{
    private readonly ILogger<InventoryConsumer> _logger;

    public InventoryConsumer(ILogger<InventoryConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdateInventoryCommand> context)
    {
        _logger.LogInformation($"[STOK SERVİSİ] Stok güncelleniyor... Sipariş no: {context.Message.OrderId}");

        await Task.Delay(1000);

        bool isStockAvailable = true;

        if (isStockAvailable)
        {
            _logger.LogInformation($"[STOK SERVİSİ] Stok BAŞARIYLA düşüldü.");
            await context.Publish(new InventoryUpdatedEvent { CorrelationId = context.Message.CorrelationId });
        }
        else
        {
            _logger.LogWarning($"[STOK SERVİSİ] Stok YETERSİZ! Stok güncellenemedi.");
            await context.Publish(new InventoryFailedEvent { CorrelationId = context.Message.CorrelationId, Reason = "Stokta yeterli ürün yok" });
        }
    }
}

}