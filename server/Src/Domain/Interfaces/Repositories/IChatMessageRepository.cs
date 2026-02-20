using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IChatMessageRepository : IBaseRepository<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> GetByRoomIdAsync(Guid roomId, int skip = 0, int take = 50);
    Task<IEnumerable<ChatMessage>> GetLatestByRoomIdAsync(Guid roomId, int count = 50);
}
