using Application.DTOs.Chat;

namespace Application.Common.Interfaces;

/// <summary>
/// Manages Server-Sent Events (SSE) connections for real-time chat messaging.
/// </summary>
public interface ISseConnectionManager
{
    /// <summary>
    /// Adds a client connection to a room.
    /// </summary>
    /// <param name="roomId">The room to connect to</param>
    /// <param name="userId">User ID allows one connection to room per user.</param>
    /// <param name="writer">The StreamWriter for sending SSE messages</param>
    /// <param name="cancellationToken">Token to detect client disconnect</param>
    Task AddConnectionAsync(Guid roomId, Guid userId, StreamWriter writer, CancellationToken cancellationToken);
    
    /// <summary>
    /// Removes a specific connection from a room.
    /// </summary>
    Task RemoveConnectionAsync(Guid roomId, Guid connectionId);

    /// <summary>
    /// Broadcasts a message to all connected clients in a room.
    /// </summary>
    Task BroadcastToRoomAsync(Guid roomId, ChatMessageDto message);
    
    /// <summary>
    /// Gets the count of active connections in a room.
    /// </summary>
    int GetConnectionCount(Guid roomId);
}
