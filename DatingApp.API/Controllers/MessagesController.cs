using Microsoft.AspNetCore.Mvc;

using DatingApp.API.Base;
using DatingApp.API.DTOs;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Controllers;

public class MessagesController : BaseApiController
{
    private readonly IMembersRepository _membersRepository;
    private readonly IMessagesRepository _messagesRepository;

    public MessagesController(IMembersRepository membersRepository, IMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository
            ?? throw new ArgumentNullException(nameof(messagesRepository));
        _membersRepository = membersRepository
            ?? throw new ArgumentNullException(nameof(membersRepository));
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(MessageForCreationDto messageForCreationDto)
    {
        Member? sender = await _membersRepository.GetMemberByIdAsync(User.GetMemberId());
        Member? recipient = await _membersRepository.GetMemberByIdAsync(messageForCreationDto.RecipientId);

        if (sender == null || recipient == null || sender.Id == messageForCreationDto.RecipientId)
        {
            return BadRequest("Cannot send this message");
        }

        Message message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = messageForCreationDto.Content
        };

        _messagesRepository.AddMessage(message);
        if (!await _messagesRepository.SaveAllAsync())
        {
            return BadRequest("Failed to send message!");
        }

        return Ok(message.ToMessageDto());
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDto>>>
        GetMessagesByContainer([FromQuery] MessagesParameters messageParameters)
    {
        messageParameters.MemberId = User.GetMemberId();

        PagedList<Message> paginatedMessages = await _messagesRepository
            .GetMessagesForMemberAsync(messageParameters);

        AddPaginationHeader(paginatedMessages);
        return Ok(paginatedMessages.ToMessagesDto());
    }

    [HttpGet("thread/{otherMemberId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string otherMemberId)
    {
        IReadOnlyList<Message> messages = await _messagesRepository
            .GetMessageThreadAsync(User.GetMemberId(), otherMemberId);

        return Ok(messages.ToMessagesDto());
    }

    [HttpDelete("{messageId}")]
    public async Task<ActionResult> DeleteMessage(string messageId)
    {
        string memberId = User.GetMemberId();
        Message? message = await _messagesRepository.GetMessageAsync(messageId);

        if (message == null) return NotFound("Cannot delete this message");
        if (memberId != message.SenderId && memberId != message.RecipientId)
        {
            return BadRequest("You cannot delete this message");
        }

        if (memberId == message.SenderId)
            message.SenderDeleted = true;
        else if (memberId == message.RecipientId)
            message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
        {
            _messagesRepository.DeleteMessage(message);
        }

        if (!await _messagesRepository.SaveAllAsync())
        {
            return BadRequest("Problem deleting the message!");
        }

        return NoContent();
    }
}
