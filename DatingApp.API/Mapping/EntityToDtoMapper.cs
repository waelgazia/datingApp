using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Mapping;

public static class EntityToDtoMapper
{
    public static UserDto ToUserDto(this AppUser appUser, ITokenService tokenService)
    {
        return new UserDto
        {
            Id = appUser.Id,
            Email = appUser.Email,
            DisplayName = appUser.DisplayName,
            Token = tokenService.CreateToken(appUser),
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
        List<MemberDto> memberDtos = new List<MemberDto>();
        foreach (Member member in members)
        {
            memberDtos.Add(member.ToMemberDto());
        }
        return memberDtos;
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
        List<PhotoDto> photoDtos = new List<PhotoDto>();
        foreach (Photo photo in photos)
        {
            photoDtos.Add(photo.ToPhotoDto());
        }
        return photoDtos;
    }
}
