using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PaymentApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentApi.Service
{
    public class OrderMessageConsumer : BackgroundService
    {
        private readonly ILogger<OrderMessageConsumer> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly string _queueName;

    public OrderMessageConsumer(
        ILogger<OrderMessageConsumer> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _queueName = configuration["RabbitMQ:QueueName"] ?? "order-payment-queue";
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🎧 OrderMessageConsumer başlatılıyor...");

        try
        {
            InitializeRabbitMQ();
            StartConsuming(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ RabbitMQ bağlantı hatası!");
        }

        await Task.CompletedTask;
    }


    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _logger.LogInformation("✅ RabbitMQ bağlantısı başarılı. Queue: {QueueName}", _queueName);
    }
     private void StartConsuming(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                var orderMessage = JsonSerializer.Deserialize<OrderCreatedMessage>(message);

                if (orderMessage != null)
                {
                    _logger.LogInformation("📩 Mesaj alındı: Sipariş #{SiparisId} - {UrunAdi} - {ToplamTutar:C}",
                        orderMessage.SiparisId,
                        orderMessage.UrunAdi,
                        orderMessage.ToplamTutar);

                    await ProcessPaymentAsync(orderMessage, stoppingToken);

                    _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogInformation("✅ Ödeme işlendi ve mesaj onaylandı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Mesaj işleme hatası!");
                _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel?.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer
        );

        _logger.LogInformation("🎧 Mesaj dinleme başladı...");
    }
 private async Task ProcessPaymentAsync(OrderCreatedMessage order, CancellationToken cancellationToken)
    {
        _logger.LogInformation("💳 Ödeme işleniyor... Tutar: {Tutar:C}", order.ToplamTutar);
        
        await Task.Delay(1000, cancellationToken); 
        
        _logger.LogInformation("✅ Ödeme başarılı! Sipariş #{SiparisId}", order.SiparisId);
    }
    }
}