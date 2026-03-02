using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Models;
using OrderApi.Services;
using OrderApi.Services.MessageBrokerService;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService           _odemeServisi;
        private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService odemeServisi,IMessagePublisher messagePublisher, ILogger<OrderController> logger)
    {
        _odemeServisi = odemeServisi;
        _messagePublisher = messagePublisher;
        _logger       = logger;
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrderDTO dto)
    {
        var siparisId = Guid.NewGuid();
        _logger.LogInformation("📦 Sipariş oluşturuldu — SiparisId: {SiparisId}", siparisId);
            var orderMessage = new OrderCreatedMessage
        {
            SiparisId = int.Parse(siparisId.ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber),
            UrunAdi = $"Sipariş {dto.MusteriId}",
            Miktar = 1,
            Fiyat = dto.Tutar,
            ToplamTutar = dto.Tutar,
            OlusturulmaTarihi = DateTime.Now
        };
        
        try
        {
            _messagePublisher.PublishOrderCreated(orderMessage);
            _logger.LogInformation("📨 Sipariş mesajı RabbitMQ'ya gönderildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ RabbitMQ mesaj gönderme hatası");
        }
        var odemeSonuc = await _odemeServisi.Payment(new PaymentRequest
        {
            SiparisId = siparisId,
            Tutar     = dto.Tutar,
            KartNo    = dto.KartNo,
            MusteriId = dto.MusteriId
        });

        if (!odemeSonuc.Basarili)
        {
            _logger.LogWarning("❌ Sipariş iptal edildi — SiparisId: {SiparisId}, Sebep: {Sebep}",
                siparisId, odemeSonuc.HataMesaji);

            return BadRequest(new OrderResult
            {
                Basarili   = false,
                SiparisId  = siparisId,
                HataMesaji = odemeSonuc.HataMesaji,
                HataTipi   = odemeSonuc.HataTipi
            });
        }

        _logger.LogInformation("✅ Sipariş onaylandı — SiparisId: {SiparisId}, OdemeId: {OdemeId}",
            siparisId, odemeSonuc.OdemeId);

        return Ok(new OrderResult
        {
            Basarili  = true,
            SiparisId = siparisId,
            OdemeId   = odemeSonuc.OdemeId
        });
    }




    }
}