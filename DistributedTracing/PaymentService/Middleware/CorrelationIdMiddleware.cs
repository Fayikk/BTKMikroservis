using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Middleware
{
    public class CorrelationIdMiddleware
    {
         private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }



   public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault() 
                           ?? Guid.NewGuid().ToString();

        context.Request.Headers[CorrelationIdHeader] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        context.Items["CorrelationId"] = correlationId;

        _logger.LogInformation("Payment Service - CorrelationId: {CorrelationId} - Request: {Method} {Path}", 
            correlationId, context.Request.Method, context.Request.Path);

        await _next(context);

        _logger.LogInformation("Payment Service - CorrelationId: {CorrelationId} - Response: {StatusCode}", 
            correlationId, context.Response.StatusCode);
    }




    }
}