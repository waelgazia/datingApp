
using DatingApp.API.Base;
using DatingApp.API.Entities;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Interfaces;

public interface IMessagesRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessageAsync(string messageId);
    Task<PagedList<Message>> GetMessagesForMemberAsync(MessagesParameters messagesParameters);
    Task<IReadOnlyList<Message>> GetMessageThreadAsync(string recipientId, string senderId);

    void AddGroup(Group group);
    Task RemoveConnection(string connectionId);
    Task<Connection?> GetConnection(string connectionId);
    Task<Group?> GetMessageGroup(string groupName);
    Task<Group?> GetGroupForConnection(string connectionId);

    Task<bool> SaveAllAsync();
}
