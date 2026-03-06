using StackExchange.Redis;

public class RedisInboxService
{
     private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisInboxService> _logger;
    private readonly TimeSpan _expiration = TimeSpan.FromDays(7); 

    public RedisInboxService(IConnectionMultiplexer redis, ILogger<RedisInboxService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

     public async Task<bool> IsMessageProcessedAsync(Guid messageId)
    {
        var db = _redis.GetDatabase();
        var key = GetKey(messageId);
        
        var exists = await db.KeyExistsAsync(key);
        
        if (exists)
        {
            _logger.LogWarning("⚠️ DUPLICATE! Mesaj daha önce işlendi: {MessageId}", messageId);
        }
        else
        {
            _logger.LogInformation("✅ Yeni mesaj: {MessageId}", messageId);
        }
        
        return exists;
    }



  public async Task MarkAsProcessedAsync(Guid messageId, string messageType, object messageData)
    {
        var db = _redis.GetDatabase();
        var key = GetKey(messageId);
        
        var data = new
        {
            MessageId = messageId,
            MessageType = messageType,
            ProcessedAt = DateTime.UtcNow,
            Data = messageData
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data);
        
        await db.StringSetAsync(key, json, _expiration);
        
        _logger.LogInformation("📝 Mesaj Redis'e kaydedildi (TTL: {Expiration}): {MessageId}", 
            _expiration, messageId);
    }


   public async Task<long> GetProcessedMessageCountAsync()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: "inbox:*").ToList();
        return keys.Count;
    }


  private static string GetKey(Guid messageId) => $"inbox:{messageId}";

}