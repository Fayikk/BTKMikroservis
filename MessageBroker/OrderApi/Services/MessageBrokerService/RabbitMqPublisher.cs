using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace OrderApi.Services.MessageBrokerService
{
    public class RabbitMqPublisher : IMessagePublisher,IDisposable
    {

        private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;
        
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        _queueName = configuration["RabbitMQ:QueueName"] ?? "order-payment-queue";

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Queue oluştur
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation("RabbitMQ bağlantısı başarılı. Queue: {QueueName}", _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ bağlantı hatası!");
            throw;
        }
    }



        public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }

       public void PublishOrderCreated<T>(T message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // Mesaj kalıcılığı
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _queueName,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("✅ Mesaj gönderildi: {Message}", json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Mesaj gönderme hatası!");
            throw;
        }
    }
    }
}