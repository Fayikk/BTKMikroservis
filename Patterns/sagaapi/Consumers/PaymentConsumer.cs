using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using sagaapi.Contracts;
namespace sagaapi.Consumers
{
 public class PaymentConsumer :
    IConsumer<ProcessPaymentCommand>,
    IConsumer<CancelPaymentCommand>
{
    private readonly ILogger<PaymentConsumer> _logger;

    public PaymentConsumer(ILogger<PaymentConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        _logger.LogInformation($"[ÖDEME SERVİSİ] Ödeme işlemi başlatıldı. Sipariş no: {context.Message.OrderId}");

        await Task.Delay(1000);

        bool isSuccess = true;

        if (isSuccess)
        {
            _logger.LogInformation($"[ÖDEME SERVİSİ] Ödeme BAŞARIYLA alındı.");
            await context.Publish(new PaymentProcessedEvent { CorrelationId = context.Message.CorrelationId });
        }
        else
        {
            _logger.LogWarning($"[ÖDEME SERVİSİ] Ödeme ALINAMADI (Örn: Bakiye Yetersiz).");
            await context.Publish(new PaymentFailedEvent { CorrelationId = context.Message.CorrelationId, Reason = "Yetersiz Bakiye" });
        }
    }

    public async Task Consume(ConsumeContext<CancelPaymentCommand> context)
    {
        _logger.LogWarning($"[ÖDEME SERVİSİ - TELAFİ (COMPENSATION)] Sipariş için stok süreci başarısız olduğu için ödeme İADE ediliyor... Sipariş no: {context.Message.OrderId}");

        await Task.Delay(1000);

        _logger.LogInformation($"[ÖDEME SERVİSİ - TELAFİ] Ödeme müşteriye başarıyla İADE EDİLDİ.");
    }
}

}