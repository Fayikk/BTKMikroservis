using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace OrderApi.Services
{
    public class OrderService : IOrderService
    {
         private readonly HttpClient             _httpClient;
    private readonly ILogger<OrderService>  _logger;

    public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
    {
        _httpClient = httpClient;
        _logger     = logger;
    }

    public async Task<PaymentResult> Payment(PaymentRequest istek)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/payment", istek);
            response.EnsureSuccessStatusCode();

            var sonuc = await response.Content.ReadFromJsonAsync<PaymentResult>();
            _logger.LogInformation("✅ Ödeme başarılı — OdemeId: {OdemeId}", sonuc?.OdemeId);
            return sonuc!;
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError("🔴 [CIRCUIT AÇIK] Ödeme servisi devre dışı: {Mesaj}", ex.Message);
            return new PaymentResult
            {
                Basarili   = false,
                HataMesaji = "Ödeme sistemi şu an hizmet veremiyor. Lütfen daha sonra tekrar deneyin.",
                HataTipi   = ErrorType.ServisDevreDisi
            };
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError("⏰ [TIMEOUT] Ödeme servisi yanıt vermedi: {Mesaj}", ex.Message);
            return new PaymentResult
            {
                Basarili   = false,
                HataMesaji = "Ödeme işlemi zaman aşımına uğradı. Lütfen tekrar deneyin.",
                HataTipi   = ErrorType.ZamanAsimi
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("🌐 [NETWORK] Ödeme servisine ulaşılamıyor: {Mesaj}", ex.Message);
            return new PaymentResult
            {
                Basarili   = false,
                HataMesaji = "Ödeme servisine ulaşılamıyor.",
                HataTipi   = ErrorType.BaglantiHatasi
            };
        }
    }

    public async Task<bool> CancelPayment(Guid odemeId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/payment/{odemeId}");
            return response.IsSuccessStatusCode;
        }
        catch (BrokenCircuitException)
        {
            _logger.LogError("🔴 [CIRCUIT AÇIK] Ödeme iptali yapılamıyor — devre açık.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Ödeme iptali başarısız: {Mesaj}", ex.Message);
            return false;
        }
    }
    }
}