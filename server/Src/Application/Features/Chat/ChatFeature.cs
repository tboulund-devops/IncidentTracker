using Application.Common.Interfaces;
using Application.Common.Interfaces.Features;
using Application.Common.Results;
using Application.DTOs.Chat;
using Application.DTOs.Entities;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;

namespace Application.Features.Chat;

public class ChatFeature(
    IChatMessageRepository messageRepository,
    IChatRoomRepository roomRepository,
    IUserRepository userRepository
) : IChatFeature
{
    public async Task<Result<ChatMessageDto>> CreateMessageAsync(Guid userId, SendMessageRequest request)
    {
        try
        {
            // Verify user is member of the room
            if (!await roomRepository.IsMemberAsync(request.RoomId, userId))
            {
                return Result<ChatMessageDto>.Failure("You are not a member of this room");
            }

            var message = ChatMessage.Create(request.RoomId, userId, request.Content);
            
            await messageRepository.AddAsync(message);

            var messageDto = new ChatMessageDto(
                message.Id,
                message.RoomId,
                UserDto.Map(message.Sender),
                message.Content,
                message.CreatedAt
            );

            return Result<ChatMessageDto>.Success(messageDto);
        }
        catch (EntityNotFoundException ex)
        {
            return Result<ChatMessageDto>.Failure(ex.Message);
        }
        catch (RepositoryException ex)
        {
            return Result<ChatMessageDto>.Failure($"Failed to send message: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ChatMessageDto>>> GetMessagesAsync(Guid userId, Guid roomId, int skip = 0, int take = 50)
    {
        try
        {
            if(!await roomRepository.IsMemberAsync(roomId, userId)) return Result<IEnumerable<ChatMessageDto>>.Failure("You are not a member of this room", ResultStatus.Unauthorized);
            
            var messages = await messageRepository.GetByRoomIdAsync(roomId, skip, take);
            
            var messageDtos = messages.Select(m => new ChatMessageDto(
                m.Id,
                m.RoomId,
                UserDto.Map(m.Sender),
                m.Content,
                m.CreatedAt
            ));

            return Result<IEnumerable<ChatMessageDto>>.Success(messageDtos);
        }
        catch (RepositoryException ex)
        {
            return Result<IEnumerable<ChatMessageDto>>.Failure($"Failed to get messages: {ex.Message}");
        }
    }

    public async Task<Result<ChatRoomDto>> CreateRoomAsync(Guid userId, CreateRoomRequest request)
    {
        try
        {
            var room = ChatRoom.Create(request.Name, userId, request.Description);
            
            await roomRepository.AddAsync(room);
            
            return Result<ChatRoomDto>.Success(ChatRoomDto.Map(room));
        }
        catch (RepositoryException ex)
        {
            return Result<ChatRoomDto>.Failure($"Failed to create room: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ChatRoomDto>>> GetUserRoomsAsync(Guid userId)
    {
        try
        {
            var rooms = await roomRepository.GetRoomsForUserAsync(userId);
            
            var roomsDto = rooms.Select(ChatRoomDto.Map);
            
            return Result<IEnumerable<ChatRoomDto>>.Success(roomsDto);
        }
        catch (RepositoryException ex)
        {
            return Result<IEnumerable<ChatRoomDto>>.Failure($"Failed to get rooms: {ex.Message}");
        }
    }

    public async Task<Result> JoinRoomAsync(Guid userId, Guid roomId)
    {
        try
        {
            var room = await roomRepository.FindByIdAsync(roomId);
            
            if (await roomRepository.IsMemberAsync(roomId, userId))
            {
                return Result.Failure("You are already a member of this room");
            }

            await roomRepository.AddMemberAsync(roomId, userId);
            return Result.Success("Successfully joined the room");
        }
        catch (EntityNotFoundException)
        {
            return Result.Failure("Room not found");
        }
        catch (RepositoryException ex)
        {
            return Result.Failure($"Failed to join room: {ex.Message}");
        }
    }

    public async Task<Result> LeaveRoomAsync(Guid userId, Guid roomId)
    {
        try
        {
            if (!await roomRepository.IsMemberAsync(roomId, userId))
            {
                return Result.Failure("You are not a member of this room");
            }

            await roomRepository.RemoveMemberAsync(roomId, userId);
            // Note: SSE connections are managed by connectionId, not userId
            // Active connections will fail authorization on next request
            
            return Result.Success();
        }
        catch (RepositoryException ex)
        {
            return Result.Failure($"Failed to leave room: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ChatRoomDto>>> SearchRoomByNameAsync(string name)
    {
        var rooms = await roomRepository.SearchRoomsByNameAsync(name);
        var roomsDto = rooms.Select(ChatRoomDto.Map).ToList();
        return roomsDto.Count == 0 ? Result<IEnumerable<ChatRoomDto>>.Failure("No rooms found") : Result<IEnumerable<ChatRoomDto>>.Success(roomsDto);
    }
}
