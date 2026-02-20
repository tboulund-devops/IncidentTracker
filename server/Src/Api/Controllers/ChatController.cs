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
    IChatRoomRepository roomRepository,
    ISimpleSse backplane
) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// SSE endpoint to subscribe to real-time messages in a chat room.
    [HttpGet("stream")]
    public async Task StreamMessages(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var (connectionId, channel) = backplane.Connect();

        try
        {
            // 🔥 Configure SSE headers manually
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("X-Accel-Buffering", "no"); // important for nginx

            await Response.Body.FlushAsync(cancellationToken);

            // Subscribe to rooms
            var clientRooms = await roomRepository.GetRoomsForUserAsync(userId);
            foreach (var room in clientRooms)
            {
                await backplane.AddToGroupAsync(connectionId, room.Id);
            }

            // Send connected event
            await WriteSseEvent("connected", 
                $"Connection: {connectionId}", 
                cancellationToken);

            // Optional heartbeat
            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await WriteSseEvent("ping", "keep-alive", cancellationToken);
                    await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                }
            }, cancellationToken);

            // 🔥 Main channel loop
            await foreach (var evt in channel.Reader.ReadAllAsync(cancellationToken))
            {
                await WriteSseEvent(
                    evt.Group.ToString()!,
                    evt.Data.GetRawText(),
                    cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // client disconnected — normal
        }
        finally
        {
            await backplane.DisconnectAsync(connectionId);
        }
    }
    
    private async Task WriteSseEvent(
        string eventName,
        string data,
        CancellationToken cancellationToken)
    {
        await Response.WriteAsync($"event: {eventName}\n", cancellationToken);
        await Response.WriteAsync($"data: {data}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
    
    /// <summary>
    /// Send a message to a chat room.
    /// </summary>
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        await backplane.SendToGroupAsync(request.RoomId, request);
        var result = await chatFeature.CreateMessageAsync(GetUserId(), request);
        
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
        throw new NotImplementedException();
    }
}
