using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatRoomRepository(MyDbContext dbContext) : IChatRoomRepository
{
    public async Task<ChatRoom> AddAsync(ChatRoom entity)
    {
        try
        {
            var created = await dbContext.ChatRooms.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return created.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public Task<bool> DeleteAsync(ChatRoom entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ChatRoom> FindByIdAsync(Guid id)
    {
        var room = await dbContext.ChatRooms
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        return room ?? throw new EntityNotFoundException("Chat room not found");
    }

    public Task<IEnumerable<ChatRoom>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(ChatRoom entity)
    {
        try
        {
            var existing = await FindByIdAsync(entity.Id);
            entity = entity with { UpdatedAt = DateTime.UtcNow };
            dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to update chat room: {e.Message}", e);
        }
    }

    public async Task<ChatRoom?> GetByIdWithMembersAsync(Guid roomId)
    {
        return await dbContext.ChatRooms
            .Include(r => r.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);
    }

    public async Task<IEnumerable<ChatRoom>> GetRoomsForUserAsync(Guid userId)
    {
        return await dbContext.ChatRoomMembers
            .Where(m => m.UserId == userId)
            .Include(m => m.Room)
            .ThenInclude(r => r.Members)
            .Select(m => m.Room)
            .Where(r => !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> AddMemberAsync(Guid roomId, Guid userId)
    {
        try
        {
            var member = ChatRoomMember.Create(roomId, userId);
            await dbContext.ChatRoomMembers.AddAsync(member);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to add member to room: {e.Message}", e);
        }
    }

    public async Task<bool> RemoveMemberAsync(Guid roomId, Guid userId)
    {
        try
        {
            var member = await dbContext.ChatRoomMembers
                .FirstOrDefaultAsync(m => m.RoomId == roomId && m.UserId == userId);

            if (member != null)
            {
                dbContext.ChatRoomMembers.Remove(member);
                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException($"Failed to remove member from room: {e.Message}", e);
        }
    }

    public async Task<bool> IsMemberAsync(Guid roomId, Guid userId)
    {
        return await dbContext.ChatRoomMembers
            .AnyAsync(m => m.RoomId == roomId && m.UserId == userId);
    }
}
