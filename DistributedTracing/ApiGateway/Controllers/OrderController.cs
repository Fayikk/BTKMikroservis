using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
          private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrderController> _logger;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public OrderController(IHttpClientFactory httpClientFactory, ILogger<OrderController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

      [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        
        _logger.LogInformation("API Gateway - CorrelationId: {CorrelationId} - Creating order for customer: {CustomerId}", 
            correlationId, request.CustomerId);

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add(CorrelationIdHeader, correlationId);

        var orderServiceUrl = Environment.GetEnvironmentVariable("ORDER_SERVICE_URL") ?? "http://localhost:5001";
        var response = await client.PostAsJsonAsync($"{orderServiceUrl}/api/orders", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
            
            _logger.LogInformation("API Gateway - CorrelationId: {CorrelationId} - Order created successfully: {OrderId}", 
                correlationId, result?.OrderId);
            
            return Ok(result);
        }

        _logger.LogError("API Gateway - CorrelationId: {CorrelationId} - Failed to create order. Status: {StatusCode}", 
            correlationId, response.StatusCode);
        
        return StatusCode((int)response.StatusCode, "Failed to create order");
    }
    
         [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(string orderId)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        
        _logger.LogInformation("API Gateway - CorrelationId: {CorrelationId} - Getting order: {OrderId}", 
            correlationId, orderId);

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add(CorrelationIdHeader, correlationId);

        var orderServiceUrl = Environment.GetEnvironmentVariable("ORDER_SERVICE_URL") ?? "http://localhost:5001";
        var response = await client.GetAsync($"{orderServiceUrl}/api/orders/{orderId}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
            return Ok(result);
        }

        return StatusCode((int)response.StatusCode, "Failed to get order");
    }
    
    
    }
}


public record CreateOrderRequest(string CustomerId, decimal Amount, string Description);
public record OrderResponse(string OrderId, string CustomerId, decimal Amount, string Status, string PaymentStatus);
