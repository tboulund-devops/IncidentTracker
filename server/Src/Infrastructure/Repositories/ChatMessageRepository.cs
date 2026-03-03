using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatMessageRepository(MyDbContext dbContext) : IChatMessageRepository
{
    public async Task<ChatMessage> AddAsync(ChatMessage entity)
    {
        try
        {
            var created = await dbContext.ChatMessages.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return created.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public Task<bool> DeleteAsync(ChatMessage entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ChatMessage> FindByIdAsync(Guid id)
    {
        var message = await dbContext.ChatMessages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == id);

        return message ?? throw new EntityNotFoundException("Chat message not found");
    }

    public Task<IEnumerable<ChatMessage>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(ChatMessage entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ChatMessage>> GetByRoomIdAsync(Guid roomId, int skip = 0, int take = 50)
    {
        return await dbContext.ChatMessages
            .Where(m => m.RoomId == roomId && !m.IsDeleted)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(take)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetLatestByRoomIdAsync(Guid roomId, int count = 50)
    {
        return await GetByRoomIdAsync(roomId, 0, count);
    }
}
