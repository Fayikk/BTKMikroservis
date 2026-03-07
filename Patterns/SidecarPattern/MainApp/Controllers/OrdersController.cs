using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers
{

public record Order(string Id, string Urun, int Adet, string Durum);

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
          private static readonly Dictionary<string, Order> _orders = [];
        private readonly FileLogger _logger;

        public OrdersController(FileLogger logger) => _logger = logger;

         [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest req)
    {
        var order = new Order("Guid.NewGuid()", req.Urun, req.Adet, "Beklemede");
        _orders[order.Id] = order;

        _logger.Info("Sipariş oluşturuldu", new { order.Id, order.Urun, order.Adet });

        return Ok();
    }


    [HttpGet]
    [ResponseCache(Duration = 604800,Location = ResponseCacheLocation.Any)]
      public IActionResult GetAllOrders()
    {
   
        return Ok(new List<Order>(){new Order("1","test",1,"test"),new Order("2","test",1,"test")});
    }

    }
}


public record CreateOrderRequest(string Urun,int Adet);