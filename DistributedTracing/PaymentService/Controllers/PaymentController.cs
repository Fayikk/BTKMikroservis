using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
    private static readonly Dictionary<string, Payment> Payments = new();

    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }


          [HttpPost("process")]
    public IActionResult ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var paymentId = Guid.NewGuid().ToString();
        
        _logger.LogInformation("Payment Service - CorrelationId: {CorrelationId} - Processing payment: {PaymentId} for order: {OrderId}, Amount: {Amount}", 
            correlationId, paymentId, request.OrderId, request.Amount);

        var random = new Random();
        var isSuccess = random.Next(100) < 80;

        var payment = new Payment
        {
            PaymentId = paymentId,
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Status = isSuccess ? "Success" : "Failed",
            ProcessedAt = DateTime.UtcNow
        };

        Payments[paymentId] = payment;

        _logger.LogInformation("Payment Service - CorrelationId: {CorrelationId} - Payment processed: {PaymentId}, Status: {Status}", 
            correlationId, paymentId, payment.Status);

        if (isSuccess)
        {
            return Ok(new PaymentResponse(payment.PaymentId, payment.Status, "Payment processed successfully"));
        }

        return BadRequest(new PaymentResponse(payment.PaymentId, payment.Status, "Payment processing failed"));
    }

          [HttpGet("{paymentId}")]
    public IActionResult GetPayment(string paymentId)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        
        _logger.LogInformation("Payment Service - CorrelationId: {CorrelationId} - Getting payment: {PaymentId}", 
            correlationId, paymentId);

        if (Payments.TryGetValue(paymentId, out var payment))
        {
            return Ok(new PaymentResponse(payment.PaymentId, payment.Status, "Payment found"));
        }

        _logger.LogWarning("Payment Service - CorrelationId: {CorrelationId} - Payment not found: {PaymentId}", 
            correlationId, paymentId);
        
        return NotFound($"Payment {paymentId} not found");
    }
    }

    
}


public record ProcessPaymentRequest(string OrderId, string CustomerId, decimal Amount);
public record PaymentResponse(string PaymentId, string Status, string Message = "");



public class Payment
{
    public string PaymentId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}