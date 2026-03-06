using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace OrderService.Outbox
{
    public class RedisOutboxService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisOutboxService> _logger;

        public RedisOutboxService(IConnectionMultiplexer redis, ILogger<RedisOutboxService> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task<bool> SavePendingMessageAsync<T>(Guid messageId, T message) where T : class
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"outbox:{messageId}";

            var outboxEntry = new OutboxEntry
            {
                MessageId = messageId,
                MessageType = typeof(T).Name,
                MessageData = System.Text.Json.JsonSerializer.Serialize(message),
                Status = OutboxStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Attempts = 0
            };

            var json = System.Text.Json.JsonSerializer.Serialize(outboxEntry);
            var saved = await db.StringSetAsync(key, json, TimeSpan.FromDays(7));

            _logger.LogInformation("✅ Outbox'a kaydedildi: {MessageId}, Type: {Type}",
                messageId, typeof(T).Name);

            return saved;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Outbox kaydetme hatası: {MessageId}", messageId);
            return false;
        }
    }

         public async Task<bool> MarkAsSentAsync(Guid messageId)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"outbox:{messageId}";

            var json = await db.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                _logger.LogWarning("⚠️ Outbox'ta bulunamadı: {MessageId}", messageId);
                return false;
            }

            var entry = JsonSerializer.Deserialize<OutboxEntry>((string)json!);
            if (entry == null) return false;

            entry.Status = OutboxStatus.Sent;
            entry.SentAt = DateTime.UtcNow;

            await db.StringSetAsync(key, JsonSerializer.Serialize(entry), TimeSpan.FromDays(7));

            _logger.LogInformation("✅ Outbox mesajı gönderildi olarak işaretlendi: {MessageId}", messageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Outbox update hatası: {MessageId}", messageId);
            return false;
        }
    }
         
          public async Task IncrementAttemptsAsync(Guid messageId)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"outbox:{messageId}";

            var json = await db.StringGetAsync(key);
            if (json.IsNullOrEmpty) return;

            var entry = JsonSerializer.Deserialize<OutboxEntry>((string)json!);
            if (entry == null) return;

            entry.Attempts++;
            entry.LastAttemptAt = DateTime.UtcNow;

            // 10 denemeden sonra failed olarak işaretle
            if (entry.Attempts >= 10)
            {
                entry.Status = OutboxStatus.Failed;
                _logger.LogError("❌ Mesaj 10 denemeden sonra failed: {MessageId}", messageId);
            }

            await db.StringSetAsync(key, JsonSerializer.Serialize(entry), TimeSpan.FromDays(7));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Attempt increment hatası: {MessageId}", messageId);
        }
    }


             public async Task<List<OutboxEntry>> GetPendingMessagesAsync()
    {
        try
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());

            var keys = server.Keys(pattern: "outbox:*").ToList();
            var pendingMessages = new List<OutboxEntry>();

            foreach (var key in keys)
            {
                var json = await db.StringGetAsync(key);
                if (json.IsNullOrEmpty) continue;

                var entry = JsonSerializer.Deserialize<OutboxEntry>((string)json!);
                if (entry?.Status == OutboxStatus.Pending)
                {
                    pendingMessages.Add(entry);
                }
            }

            return pendingMessages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Pending mesajları getirme hatası");
            return new List<OutboxEntry>();
        }
    }
    public async Task<OutboxStats> GetStatsAsync()
    {
        try
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());

            var keys = server.Keys(pattern: "outbox:*").ToList();
            var stats = new OutboxStats();

            foreach (var key in keys)
            {
                var json = await db.StringGetAsync(key);
                if (json.IsNullOrEmpty) continue;

                var entry = System.Text.Json.JsonSerializer.Deserialize<OutboxEntry>(json.ToString());
                if (entry == null) continue;

                stats.Total++;
                switch (entry.Status)
                {
                    case OutboxStatus.Pending:
                        stats.Pending++;
                        break;
                    case OutboxStatus.Sent:
                        stats.Sent++;
                        break;
                    case OutboxStatus.Failed:
                        stats.Failed++;
                        break;
                }
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Stats hatası");
            return new OutboxStats();
        }
    }





    }
}


public class OutboxEntry
{
    public Guid MessageId { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string MessageData { get; set; } = string.Empty;
    public OutboxStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public int Attempts { get; set; }
}

public enum OutboxStatus
{
    Pending, 
    Sent,    
    Failed   
}


public class OutboxStats
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Sent { get; set; }
    public int Failed { get; set; }
}