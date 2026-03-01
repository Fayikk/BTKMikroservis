using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutonomousService.Api.Service.AutonomousServices
{
    public interface IStockService
    {
        void CalculateStock(int Id);
            void Add(object obj);
    }

    public interface INotificationService
    {
        void SendNotify(string email);
        void Add(object obj);
    }


    public interface IOrderRepository
    {
        void Add(object order);
    }


    public class Repository
    {
        public readonly INotificationService notificationService;
        public IOrderRepository orderRepository{ get; set; }
         public readonly IStockService _stockService;
        void Add(object obj);
    }

    public class OrderService 
    {
        private readonly Repository repository = new Repository();
        private readonly IMessage _message;

        public async Task<bool> CreateOrder(object dto)
        {
            repository.orderRepository.Add(dto);
            //Stock Service
            // await _message.PublishAsync(new StockCreated({
            //     OrderId: dto.Id,
            //     OrderName:dto.Name,
            // }));

        }
    }


    public class StockConsumer
    {
        public async Task Handle(object e)
        {
            var stock = await repository.orderRepository.Get(e);
        }
    }
}