using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Features;
using Application.Common.Results;
using Application.DTOs.Chat;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController(
    IChatFeature chatFeature,
    ISseConnectionManager sseConnectionManager,
    IChatRoomRepository roomRepository
) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// SSE endpoint to subscribe to real-time messages in a chat room.
    /// </summary>
    [HttpGet("rooms/{roomId:guid}/stream")]
    public async Task StreamMessages(Guid roomId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        
        // Verify user is member of the room
        if (!await roomRepository.IsMemberAsync(roomId, userId))
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no"); // Disable nginx buffering

        await Response.Body.FlushAsync(cancellationToken);
        
        // Do NOT use AutoFlush - it causes synchronous operations which are disallowed
        var writer = new StreamWriter(Response.Body);

        // Send initial connection event
        await writer.WriteAsync($"event: connected\ndata: {{\"roomId\": \"{roomId}\"}}\n\n");
        await writer.FlushAsync(cancellationToken);

        // Register connection
        await sseConnectionManager.AddConnectionAsync(roomId, userId, writer, cancellationToken);

        try
        {
            // Keep connection alive with heartbeat
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                await writer.WriteAsync(": heartbeat\n\n");
                await writer.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected - this is expected
        }
        finally
        {
            await sseConnectionManager.RemoveConnectionAsync(roomId, userId);
        }
    }

    /// <summary>
    /// Send a message to a chat room.
    /// </summary>
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var result = await chatFeature.SendMessageAsync(GetUserId(), request);
        
        return result.Status switch
        {
            ResultStatus.Success => Ok(result),
            ResultStatus.Failure => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Get message history for a room.
    /// </summary>
    [HttpGet("rooms/{roomId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid roomId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var userId = GetUserId();
        
        if (!await roomRepository.IsMemberAsync(roomId, userId))
        {
            return Forbid("You are not a member of this room");
        }

        var result = await chatFeature.GetMessagesAsync(roomId, skip, take);
        
        return result.Status switch
        {
            ResultStatus.Success => Ok(result),
            ResultStatus.Failure => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Create a new chat room.
    /// </summary>
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
    {
        var result = await chatFeature.CreateRoomAsync(GetUserId(), request);
        
        return result.Status switch
        {
            ResultStatus.Success => CreatedAtAction(nameof(GetMessages), new { roomId = result.Dto!.Id }, result.Message),
            ResultStatus.Failure => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Get all rooms the user is a member of.
    /// </summary>
    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms()
    {
        var result = await chatFeature.GetUserRoomsAsync(GetUserId());
        
        return result.Status switch
        {
            ResultStatus.Success => Ok(result),
            ResultStatus.Failure => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Join a chat room.
    /// </summary>
    [HttpPost("rooms/{roomId:guid}/join")]
    public async Task<IActionResult> JoinRoom(Guid roomId)
    {
        var result = await chatFeature.JoinRoomAsync(GetUserId(), roomId);
        
        return result.Status switch
        {
            ResultStatus.Success => Ok(result),
            ResultStatus.Failure => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Leave a chat room.
    /// </summary>
    [HttpPost("rooms/{roomId:guid}/leave")]
    public async Task<IActionResult> LeaveRoom(Guid roomId)
    {
        var result = await chatFeature.LeaveRoomAsync(GetUserId(), roomId);
        
        return result.Status switch
        {
            ResultStatus.Success => Ok(result),
            ResultStatus.Failure => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Get active connection count for a room (admin/debug).
    /// </summary>
    [HttpGet("rooms/{roomId:guid}/connections")]
    public IActionResult GetConnectionCount(Guid roomId)
    {
        var count = sseConnectionManager.GetConnectionCount(roomId);
        return Ok(new { roomId, activeConnections = count });
    }
}
