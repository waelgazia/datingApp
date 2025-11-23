using Microsoft.EntityFrameworkCore;

using DatingApp.API.Base;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Data;

public class MessagesRepository(AppDbContext _dbContext) : IMessagesRepository
{
    public void AddMessage(Message message)
    {
        _dbContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _dbContext.Messages.Remove(message);
    }

    public async Task<Message?> GetMessageAsync(string messageId)
    {
        return await _dbContext.Messages.FindAsync(messageId);
    }

    public async Task<PagedList<Message>> GetMessagesForMemberAsync(MessagesParameters messagesParameters)
    {
        IQueryable<Message> messagesQuery = _dbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .OrderByDescending(m => m.SentAt)       /* order message by the newest */
            .AsQueryable();

        messagesQuery = messagesParameters.Container.ToLowerInvariant() switch
        {
            MessagesContainer.OUTBOX => messagesQuery
                .Where(m => m.SenderId == messagesParameters.MemberId && !m.SenderDeleted),
            _ /* INBOX */ => messagesQuery
                .Where(m => m.RecipientId == messagesParameters.MemberId && !m.RecipientDeleted)
        };

        return await PagedList<Message>.CreateAsync(
            messagesQuery,
            messagesParameters.PageNumber,
            messagesParameters.PageSize);
    }

    /// <summary>
    /// Returns messages thread between two members.
    /// </summary>
    /// <param name="loggedMemberId">The logged in member id</param>
    /// <param name="otherMemberId">The other member id</param>
    public async Task<IReadOnlyList<Message>>
        GetMessageThreadAsync(string loggedMemberId, string otherMemberId)
    {
        // mark the message as read if the recipient fetches it
        await _dbContext.Messages
            .Where(m => m.RecipientId == loggedMemberId && m.SenderId == otherMemberId && m.ReadAt == null)
            .ExecuteUpdateAsync(setter => setter.SetProperty(m => m.ReadAt, DateTime.UtcNow));

        return await _dbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Where(m =>
                   (m.RecipientId == loggedMemberId && m.SenderId == otherMemberId && !m.RecipientDeleted)
                || (m.RecipientId == otherMemberId && m.SenderId == loggedMemberId && !m.SenderDeleted))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dbContext.SaveChangesAsync() >= 0;
    }
}
