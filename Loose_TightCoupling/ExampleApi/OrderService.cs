using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi
{

    public interface IOrderService
    {
        Task<bool> Payment(decimal amount);
    }
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Payment(decimal amount)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5136/CreatePayment",new {amount = amount});
            return true;
        }
    }
}