public class ProxyService
{
     private readonly HttpClient _httpClient;
        private readonly ILogger<ProxyService> _logger;

        public ProxyService(HttpClient httpClient, ILogger<ProxyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
  public async Task<HttpResponseMessage> ForwardRequest(
            string targetUrl,
            HttpMethod method,
            HttpContext context,
            object? body = null)
        {
            var request = new HttpRequestMessage(method, targetUrl);

            if (context.Request.Headers.TryGetValue("X-Gateway-Secret", out var gatewaySecret))
                request.Headers.Add("X-Gateway-Secret", gatewaySecret.ToString());

            if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
                request.Headers.Add("X-User-Id", userId.ToString());

            if (context.Request.Headers.TryGetValue("X-User-Name", out var userName))
                request.Headers.Add("X-User-Name", userName.ToString());

            if (context.Request.Headers.TryGetValue("X-User-Email", out var userEmail))
                request.Headers.Add("X-User-Email", userEmail.ToString());

            if (context.Request.Headers.TryGetValue("X-User-Role", out var userRole))
                request.Headers.Add("X-User-Role", userRole.ToString());

            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                request.Headers.Add("Authorization", authHeader.ToString());

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            _logger.LogInformation("Forwarding {Method} request to {Url}", method, targetUrl);

            return await _httpClient.SendAsync(request);
        }
    
}