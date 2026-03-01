using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutonomousService.Api.Service
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



    public class Repository
    {
        public readonly INotificationService notificationService;
         public readonly IStockService _stockService;
        void Add(object obj);
    }

    public class OrderService
    {
        private readonly IStockService _stockService;
        private readonly Repository repository = new Repository();
        private readonly INotificationService notificationService;

        public async Task<bool> CreateOrder(object dto)
        {
            //Stock Service
            _stockService.CalculateStock(1);
        
            //Notification Service
            notificationService.SendNotify(dto.ToString());


            repository.notificationService.Add(dto);

            repository._stockService.Add(dto);

        }
    }
}