namespace Domain.Entities;

public sealed record ChatMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Guid RoomId { get; init; }
    public ChatRoom Room { get; init; } = null!;
    
    public Guid SenderId { get; init; }
    public User Sender { get; init; } = null!;
    
    public required string Content { get; init; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    
    public static ChatMessage Create(Guid roomId, Guid senderId, string content)
    {
        return new ChatMessage
        {
            RoomId = roomId,
            SenderId = senderId,
            Content = content
        };
    }
}
