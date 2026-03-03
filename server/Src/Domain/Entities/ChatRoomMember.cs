namespace Domain.Entities;

public sealed record ChatRoomMember
{
    public Guid RoomId { get; init; }
    public ChatRoom Room { get; init; } = null!;
    
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;
    
    public static ChatRoomMember Create(Guid roomId, Guid userId)
    {
        return new ChatRoomMember
        {
            RoomId = roomId,
            UserId = userId
        };
    }
}
