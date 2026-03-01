using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingleResponsibility.Api.Services
{
    public class OrderService
    {
        private readonly SMTPClient sMTPClient;
        private readonly ILogger logger;
        private readonly ApplicationDbContext applicationDbContext;
        public OrderService()
        {
            
        }


         //SRP Olmayan Örnek

         public async Task<bool> CreateOrderAsync(object order)
        {
            //Veritabanı bağlantısı

            //Log İşlemi
            logger.Log("test");

            //SMTP
            sMTPClient.Name = order.GetType().Name;
            SendMail("veznedaroglufayik2@gmail.com");
        }

        private bool SendMail(string message)
        {
            logger.Log("test", message);
            return true;
        }

        
    }

    public class SMTPClient
    {
        public string Name { get; set; } = "SMTPName";
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class Logger : ILogger
    {
        public void Log(string message)
        {
            //uzak sunucuya loglamak gerekiyorsa
        }
    }


    public class ApplicationDbContext
    {
        
    }
}