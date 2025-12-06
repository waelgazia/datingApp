using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using DatingApp.API.DTOs;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;

namespace DatingApp.API.SignalR;

[Authorize]
public class MessageHub(
    IMembersRepository _membersRepository,
    IMessagesRepository _messagesRepository,
    IHubContext<PresenceHub> _presenceHub) : Hub
{
    public async override Task OnConnectedAsync()
    {
        // create a group for user sending the request, and the other on provided in the query string
        HttpContext? httpContext = Context.GetHttpContext();
        string? otherUserId = httpContext?.Request?.Query["userId"].ToString()
            ?? throw new HubException("Cannot find other user id");

        string groupName = GetGroupName(GetUserId(), otherUserId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await _messagesRepository.GetMessageThreadAsync(GetUserId(), otherUserId);

        await Clients.Group(groupName).SendAsync("ReceiveMessagesThread", messages.ToMessagesDto());
    }

    /// <summary>
    /// This message will be called from the client by its name (SendMessage)
    /// </summary>
    public async Task SendMessage(MessageForCreationDto createMessageDto)
    {
        Member? sender = await _membersRepository.GetMemberByIdAsync(GetUserId());
        Member? recipient = await _membersRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (sender == null || recipient == null || sender.Id == createMessageDto.RecipientId)
        {
            throw new HubException("Cannot send this message");
        }

        Message message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        // check if the recipient is online (by checking if he is in a chat group)
        // to set the read at property.
        string groupName = GetGroupName(sender.Id, recipient.Id);
        Group? group = await _messagesRepository.GetMessageGroup(groupName);
        bool isRecipientConnectedToMessageGroup = group != null && group.Connections.Any(c => c.UserId == recipient.Id);

        if (isRecipientConnectedToMessageGroup) message.ReadAt = DateTime.UtcNow;

        _messagesRepository.AddMessage(message);
        if (await _messagesRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", message.ToMessageDto());

            // notify the recipient that a new message arrived if he is online,
            // and not connected to the message group with the sender.
            List<string> connectionIds = await PresenceTracker.GetUserConnections(recipient.Id);
            if (connectionIds != null && connectionIds.Count > 0 && !isRecipientConnectedToMessageGroup)
            {
                // send the recipient notification on all the connections (he can be connected
                // using multiple devices).
                await _presenceHub.Clients.Clients(connectionIds)
                    .SendAsync("NewMessageReceived", message.ToMessageDto());
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _messagesRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private string GetGroupName(string callerId, string otherId)
    {
        // ensure to return an ordered group name despite who is the callerId, and who is the otherId
        bool stringComparer = string.CompareOrdinal(callerId, otherId) < 0;
        return stringComparer ? $"{callerId}-{otherId}" : $"{otherId}-{callerId}";
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        Group? group = await _messagesRepository.GetMessageGroup(groupName);
        Connection connection = new Connection(Context.ConnectionId, GetUserId());

        if (group == null)
        {
            group = new Group(groupName);
            _messagesRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await _messagesRepository.SaveAllAsync();
    }
}
