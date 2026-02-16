using System.Collections.Concurrent;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.DTOs.Chat;
using StackExchange.Redis;

namespace Infrastructure.Sse;

/// <summary>
/// Redis-backed SSE connection manager for multi-server deployments.
/// Uses Redis Pub/Sub for cross-server message broadcasting.
/// Local ConcurrentDictionary still holds actual StreamWriter connections.
/// </summary>
public sealed class RedisSseConnectionManager(IConnectionMultiplexer redis) : ISseConnectionManager, IAsyncDisposable
{
    // Local connections - StreamWriters cannot be stored in Redis
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, StreamWriter>> _roomConnections = new();
    
    private readonly ISubscriber _subscriber = redis.GetSubscriber();
    private readonly ConcurrentDictionary<Guid, ChannelMessageQueue> _subscriptions = new();
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task AddConnectionAsync(Guid roomId, Guid userId, StreamWriter writer, CancellationToken cancellationToken)
    {
        var roomClients = _roomConnections.GetOrAdd(roomId, _ => new ConcurrentDictionary<Guid, StreamWriter>());
        
        // Remove existing connection with same ID if any (reconnect scenario)
        if (roomClients.TryRemove(userId, out var existingWriter))
        {
            try { await existingWriter.DisposeAsync(); } catch { /* ignore */ }
        }
        
        roomClients.TryAdd(userId, writer);
        
        // Subscribe to Redis channel for this room if first connection
        await EnsureRoomSubscriptionAsync(roomId);
        
        // Register cleanup when connection is cancelled
        cancellationToken.Register(() => RemoveConnectionAsync(roomId, userId).ConfigureAwait(false));
    }

    public async Task RemoveConnectionAsync(Guid roomId, Guid connectionId)
    {
        if (_roomConnections.TryGetValue(roomId, out var roomClients))
        {
            if (roomClients.TryRemove(connectionId, out var writer))
            {
                try { await writer.DisposeAsync(); } catch { /* ignore */ }
            }
            
            // Clean up empty rooms and unsubscribe from Redis
            if (roomClients.IsEmpty)
            {
                _roomConnections.TryRemove(roomId, out _);
                await UnsubscribeFromRoomAsync(roomId);
            }
        }
    }

    /// <summary>
    /// Publishes message to Redis Pub/Sub.
    /// All subscribed server instances will receive and broadcast to their local clients.
    /// </summary>
    public async Task BroadcastToRoomAsync(Guid roomId, ChatMessageDto message)
    {
        var channel = GetChannelName(roomId);
        var json = JsonSerializer.Serialize(message, JsonOptions);
        
        // Publish to Redis - all server instances subscribed to this room will receive
        await _subscriber.PublishAsync(RedisChannel.Literal(channel), json);
    }

    public int GetConnectionCount(Guid roomId)
    {
        // Note: This only returns LOCAL connection count
        // For total count across all servers, you'd need a separate Redis counter
        return _roomConnections.TryGetValue(roomId, out var roomClients) 
            ? roomClients.Count 
            : 0;
    }

    /// <summary>
    /// Subscribes to Redis channel for a room if not already subscribed.
    /// </summary>
    private async Task EnsureRoomSubscriptionAsync(Guid roomId)
    {
        if (_subscriptions.ContainsKey(roomId))
            return;

        var channel = GetChannelName(roomId);
        var messageQueue = await _subscriber.SubscribeAsync(RedisChannel.Literal(channel));
        
        if (_subscriptions.TryAdd(roomId, messageQueue))
        {
            // Process messages from Redis and broadcast to local clients
            _ = Task.Run(async () =>
            {
                await foreach (var redisMessage in messageQueue)
                {
                    try
                    {
                        await BroadcastToLocalClientsAsync(roomId, redisMessage.Message!);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[RedisSse] Error broadcasting to local clients: {ex.Message}");
                    }
                }
            });
        }
    }

    /// <summary>
    /// Broadcasts message to clients connected to THIS server instance.
    /// </summary>
    private async Task BroadcastToLocalClientsAsync(Guid roomId, string jsonMessage)
    {
        if (!_roomConnections.TryGetValue(roomId, out var roomClients))
            return;

        var sseMessage = $"event: message\ndata: {jsonMessage}\n\n";
        var disconnectedConnections = new List<Guid>();

        foreach (var (connectionId, writer) in roomClients)
        {
            try
            {
                await writer.WriteAsync(sseMessage);
                await writer.FlushAsync();
            }
            catch
            {
                disconnectedConnections.Add(connectionId);
            }
        }

        foreach (var connectionId in disconnectedConnections)
        {
            await RemoveConnectionAsync(roomId, connectionId);
        }
    }

    private async Task UnsubscribeFromRoomAsync(Guid roomId)
    {
        if (_subscriptions.TryRemove(roomId, out var messageQueue))
        {
            await messageQueue.UnsubscribeAsync();
        }
    }

    private static string GetChannelName(Guid roomId) => $"chat:room:{roomId}";

    public async ValueTask DisposeAsync()
    {
        foreach (var (roomId, _) in _subscriptions)
        {
            await UnsubscribeFromRoomAsync(roomId);
        }
        
        foreach (var (_, roomClients) in _roomConnections)
        {
            foreach (var (_, writer) in roomClients)
            {
                try { await writer.DisposeAsync(); } catch { /* ignore */ }
            }
        }
        
        _roomConnections.Clear();
    }
}
