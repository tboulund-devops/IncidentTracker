namespace Application.DTOs.Chat;

public record ChatMessageDto(
    Guid Id,
    Guid RoomId,
    Guid SenderId,
    string SenderName,
    string Content,
    DateTime CreatedAt
);

public record ChatRoomDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int MemberCount
);

public record SendMessageRequest(
    Guid RoomId,
    string Content
);

public record CreateRoomRequest(
    string Name,
    string? Description
);

public record JoinRoomRequest(
    Guid RoomId
);
