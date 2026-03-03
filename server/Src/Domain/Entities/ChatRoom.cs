namespace Domain.Entities;

public sealed record ChatRoom
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;
    
    public ICollection<ChatMessage> Messages { get; init; } = new List<ChatMessage>();
    public ICollection<ChatRoomMember> Members { get; init; } = new List<ChatRoomMember>();
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    public static ChatRoom Create(string name, Guid createdById, string? description = null)
    {
        return new ChatRoom
        {
            Name = name,
            Description = description,
            CreatedById = createdById
        };
    }
}
