using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Middleware
{
    public class GatewayAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _gatewaySecret;
        private readonly ILogger<GatewayAuthMiddleware> _logger;

        public GatewayAuthMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<GatewayAuthMiddleware> logger)
        {
            _next = next;
            _gatewaySecret = configuration["GATEWAY_SECRET"] ?? "GatewayInternalSecret2024!";
            _logger = logger;
        }

         public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Gateway-Secret", out var headerValue))
            {
                _logger.LogWarning("Request without Gateway Secret header rejected: {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Bu servise sadece API Gateway üzerinden erişilebilir"
                });
                return;
            }

            if (headerValue != _gatewaySecret)
            {
                _logger.LogWarning("Request with invalid Gateway Secret rejected: {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Geçersiz Gateway Secret"
                });
                return;
            }

            _logger.LogInformation("Request authenticated via Gateway: {Path}", context.Request.Path);
            await _next(context);
        }
    
    }
}