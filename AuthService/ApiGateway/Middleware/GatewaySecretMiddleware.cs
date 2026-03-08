using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Middleware
{
    public class GatewaySecretMiddleware
    {
        
        private readonly RequestDelegate _next;
        private readonly string _gatewaySecret;
        private readonly ILogger<GatewaySecretMiddleware> _logger;

        public GatewaySecretMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<GatewaySecretMiddleware> logger)
        {
            _next = next;
            _gatewaySecret = configuration["GATEWAY_SECRET"] ?? "GatewayInternalSecret2024!";
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userName = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var userEmail = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var userRole = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (!string.IsNullOrEmpty(userId))
                    context.Request.Headers["X-User-Id"] = userId;
                if (!string.IsNullOrEmpty(userName))
                    context.Request.Headers["X-User-Name"] = userName;
                if (!string.IsNullOrEmpty(userEmail))
                    context.Request.Headers["X-User-Email"] = userEmail;
                if (!string.IsNullOrEmpty(userRole))
                    context.Request.Headers["X-User-Role"] = userRole;

                _logger.LogInformation("User context added to request - UserId: {UserId}, Role: {Role}", userId, userRole);
            }

            context.Request.Headers["X-Gateway-Secret"] = _gatewaySecret;
            _logger.LogDebug("Gateway Secret header added to downstream request");

            await _next(context);
        }
    }
}