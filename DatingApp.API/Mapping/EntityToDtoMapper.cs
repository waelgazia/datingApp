using System.Linq.Expressions;

using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Mapping;

public static class EntityToDtoMapper
{
    public static async Task<UserDto> ToUserDto(this AppUser appUser, ITokenService tokenService)
    {
        return new UserDto
        {
            Id = appUser.Id,
            Email = appUser.Email!,
            DisplayName = appUser.DisplayName,
            Token = await tokenService.CreateToken(appUser),
            ImageUrl = appUser.ImageUrl
        };
    }

    public static MemberDto ToMemberDto(this Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            DateOfBirth = member.DateOfBirth,
            ImageUrl = member.ImageUrl,
            DisplayName = member.DisplayName,
            Created = member.Created,
            LastActive = member.LastActive,
            Gender = member.Gender,
            Description = member.Description,
            City = member.City,
            Country = member.Country
        };
    }

    public static IEnumerable<MemberDto> ToMembersDto(this IEnumerable<Member> members)
    {
        return members.Select(member => member.ToMemberDto());
    }

    public static PhotoDto ToPhotoDto(this Photo photo)
    {
        return new PhotoDto
        {
            Id = photo.Id,
            Url = photo.Url,
            PublicId = photo?.PublicId,
            MemberId = photo!.MemberId
        };
    }

    public static IEnumerable<PhotoDto> ToPhotosDto(this IEnumerable<Photo> photos)
    {
        return photos.Select(photo => photo.ToPhotoDto());
    }

    public static MessageDto ToMessageDto(this Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.Sender.Id,
            SenderDisplayName = message.Sender.DisplayName,
            SenderImageUrl = message.Sender.ImageUrl,
            RecipientId = message.Recipient.Id,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl,
            Content = message.Content,
            ReadAt = message.ReadAt,
            SentAt = message.SentAt
        };
    }

    /// <summary>
    /// Projection expression to be used inside IQueryable.Select().
    /// </summary>
    /// <remarks>
    /// When using IQueryable.Select(), there is no need for eagle loading (Include()) to
    /// fetch the nested entity in an entity. EF will do this in the query.
    /// </remarks>
    public static Expression<Func<Message, MessageDto>> ProjectToMessageDto(this Message message)
    {
        return message => new MessageDto
        {
            Id = message.Id,
            SenderId = message.Sender.Id,
            SenderDisplayName = message.Sender.DisplayName,
            SenderImageUrl = message.Sender.ImageUrl,
            RecipientId = message.Recipient.Id,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl,
            Content = message.Content,
            ReadAt = message.ReadAt,
            SentAt = message.SentAt
        };
    }

    public static IEnumerable<MessageDto> ToMessagesDto(this IEnumerable<Message> messages)
    {
        return messages.Select(message => message.ToMessageDto());
    }
}
