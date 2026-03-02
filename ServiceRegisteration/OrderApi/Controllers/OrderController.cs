using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Services;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService           _odemeServisi;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService odemeServisi, ILogger<OrderController> logger)
    {
        _odemeServisi = odemeServisi;
        _logger       = logger;
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrderDTO dto)
    {
        var siparisId = Guid.NewGuid();
        _logger.LogInformation("📦 Sipariş oluşturuldu — SiparisId: {SiparisId}", siparisId);

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