using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PaymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
          private readonly ILogger<PaymentController> _logger;
        private static int _requestCount = 0;
    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

[HttpPost]
    public async Task<IActionResult> Post([FromBody] PaymentRequest request)
    {
        var count = Interlocked.Increment(ref _requestCount);
        await Task.Delay(6000); 

        _logger.LogInformation("📥 Ödeme isteği #{Count} alındı — SiparisId: {SiparisId}", count, request.SiparisId);

      

        var sonuc = new PaymentResult
        {
            Basarili = true,
            OdemeId  = Guid.NewGuid()
        };

        _logger.LogInformation("✅ Ödeme başarılı #{Count} — OdemeId: {OdemeId}", count, sonuc.OdemeId);
        return Ok(sonuc);
    }




    }
}