using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MassTransit;
using Shared;

namespace OrderService.Outbox
{
    public class OutboxProcessorService : BackgroundService
    {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;

    public OutboxProcessorService(IServiceProvider serviceProvider, ILogger<OutboxProcessorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 OutboxProcessorService başlatıldı. Her 10 saniyede pending mesajları kontrol ediyor...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ OutboxProcessor hatası");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    
    private async Task ProcessPendingMessagesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxService = scope.ServiceProvider.GetRequiredService<RedisOutboxService>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var pendingMessages = await outboxService.GetPendingMessagesAsync();

        if (pendingMessages.Count == 0)
        {
            _logger.LogDebug("✅ Pending mesaj yok");
            return;
        }

        _logger.LogInformation("📨 {Count} adet pending mesaj bulundu, retry ediliyor...", pendingMessages.Count);

        foreach (var entry in pendingMessages)
        {
            try
            {
                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(entry.MessageData);
                if (message == null)
                {
                    _logger.LogWarning("⚠️ Deserialize edilemedi: {MessageId}", entry.MessageId);
                    continue;
                }

                await publishEndpoint.Publish(message);

                await outboxService.MarkAsSentAsync(entry.MessageId);

                _logger.LogInformation("✅ Pending mesaj tekrar gönderildi: {MessageId} (Attempt: {Attempts})",
                    entry.MessageId, entry.Attempts + 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Mesaj retry hatası: {MessageId}", entry.MessageId);

                await outboxService.IncrementAttemptsAsync(entry.MessageId);
            }
        }
    }


    }
}