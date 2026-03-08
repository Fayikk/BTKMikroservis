using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
          private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrdersController> _logger;
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private static readonly Dictionary<string, Order> Orders = new();

    public OrdersController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var orderId = Guid.NewGuid().ToString();
        
        _logger.LogInformation("Order Service - CorrelationId: {CorrelationId} - Creating order: {OrderId} for customer: {CustomerId}", 
            correlationId, orderId, request.CustomerId);

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add(CorrelationIdHeader, correlationId);

        var paymentServiceUrl = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_URL") ?? "http://localhost:5002";
        var paymentRequest = new { OrderId = orderId, Amount = request.Amount, CustomerId = request.CustomerId };
        
        _logger.LogInformation("Order Service - CorrelationId: {CorrelationId} - Calling Payment Service for order: {OrderId}", 
            correlationId, orderId);

        var paymentResponse = await client.PostAsJsonAsync($"{paymentServiceUrl}/api/payment/process", paymentRequest);

        string paymentStatus = "Pending";
        if (paymentResponse.IsSuccessStatusCode)
        {
            var paymentResult = await paymentResponse.Content.ReadFromJsonAsync<PaymentResult>();
            paymentStatus = paymentResult?.Status ?? "Unknown";
            
            _logger.LogInformation("Order Service - CorrelationId: {CorrelationId} - Payment processed: {PaymentId}, Status: {Status}", 
                correlationId, paymentResult?.PaymentId, paymentStatus);
        }
        else
        {
            _logger.LogWarning("Order Service - CorrelationId: {CorrelationId} - Payment failed for order: {OrderId}", 
                correlationId, orderId);
            paymentStatus = "Failed";
        }

        var order = new Order
        {
            OrderId = orderId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Description = request.Description,
            Status = paymentStatus == "Success" ? "Confirmed" : "Pending",
            PaymentStatus = paymentStatus,
            CreatedAt = DateTime.UtcNow
        };

        Orders[orderId] = order;

        _logger.LogInformation("Order Service - CorrelationId: {CorrelationId} - Order created: {OrderId}, Status: {Status}", 
            correlationId, orderId, order.Status);

        return Ok(new OrderResponse(order.OrderId, order.CustomerId, order.Amount, order.Status, order.PaymentStatus));
    }

    [HttpGet("{orderId}")]
    public IActionResult GetOrder(string orderId)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        
        _logger.LogInformation("Order Service - CorrelationId: {CorrelationId} - Getting order: {OrderId}", 
            correlationId, orderId);

        if (Orders.TryGetValue(orderId, out var order))
        {
            return Ok(new OrderResponse(order.OrderId, order.CustomerId, order.Amount, order.Status, order.PaymentStatus));
        }

        _logger.LogWarning("Order Service - CorrelationId: {CorrelationId} - Order not found: {OrderId}", 
            correlationId, orderId);
        
        return NotFound($"Order {orderId} not found");
    }



    }
}



public record CreateOrderRequest(string CustomerId, decimal Amount, string Description);
public record OrderResponse(string OrderId, string CustomerId, decimal Amount, string Status, string PaymentStatus);
public record PaymentResult(string PaymentId, string Status);

public class Order
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
