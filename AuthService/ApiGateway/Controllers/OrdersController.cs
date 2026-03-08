using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGateway.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ProxyService _proxyService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            ProxyService proxyService,
            IConfiguration configuration,
            ILogger<OrdersController> logger)
        {
            _proxyService = proxyService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.CanViewAllOrders)]
        public async Task<IActionResult> GetOrders()
        {
            var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://orderservice:8080";
            var url = $"{orderServiceUrl}/api/orders";

            _logger.LogInformation("Gateway: Fetching orders from {Url}", url);

            var response = await _proxyService.ForwardRequest(url, HttpMethod.Get, HttpContext);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.CanCreateOrder)]
        public async Task<IActionResult> CreateOrder()
        {
            var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://orderservice:8080";
            var url = $"{orderServiceUrl}/api/orders";

            _logger.LogInformation("Gateway: Fetching orders from {Url}", url);

            var response = await _proxyService.ForwardRequest(url, HttpMethod.Get, HttpContext);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}