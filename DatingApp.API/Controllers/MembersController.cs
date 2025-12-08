using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using DatingApp.API.DTOs;
using DatingApp.API.Base;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.Extensions;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Controllers;

[Authorize]
public class MembersController(IUnitOfWork _uow, IPhotoService _photoService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>>
        GetMembers([FromQuery] MembersParameters membersParameters)
    {
        membersParameters.CurrentMemberId = User.GetMemberId();
        PagedList<Member> paginatedMembers = await _uow.MembersRepository.GetMembersAsync(membersParameters);

        AddPaginationHeader(paginatedMembers);
        return Ok(paginatedMembers.ToMembersDto());
    }

    [HttpGet("{memberId}")]
    public async Task<ActionResult<MemberDto>> GetMember(string memberId)
    {
        Member? member = await _uow.MembersRepository.GetMemberByIdAsync(memberId);
        if (member == null)
        {
            return NotFound();
        }

        return Ok(member.ToMemberDto());
    }

    [HttpGet("{memberId}/photos")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetMemberPhotos(string memberId)
    {
        // return only approved images unless the for the logged in user.
        bool isLoggedInUser = User.GetMemberId() == memberId;
        IReadOnlyList<Photo> photoEntities =
            await _uow.MembersRepository.GetPhotosForMemberAsync(memberId, isLoggedInUser);

        return Ok(photoEntities.ToPhotosDto());
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberForUpdateDto memberUpdateDto)
    {
        string memberId = User.GetMemberId();
        Member? memberEntity = await _uow.MembersRepository.GetMemberForUpdateAsync(memberId);

        if (memberEntity == null)
        {
            return BadRequest("Could not get member");
        }

        memberEntity.DisplayName = memberUpdateDto.DisplayName ?? memberEntity.DisplayName;
        memberEntity.Description = memberUpdateDto.Description ?? memberEntity.Description;
        memberEntity.City = memberUpdateDto.City ?? memberEntity.City;
        memberEntity.Country = memberUpdateDto.Country ?? memberEntity.Country;

        memberEntity.User.DisplayName = memberUpdateDto.DisplayName ?? memberEntity.User.DisplayName;

        if (!await _uow.Complete())
        {
            return BadRequest("Failed to update member!");
        }

        return NoContent();
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] IFormFile file)
    {
        Member? member = await _uow.MembersRepository.GetMemberForUpdateAsync(User.GetMemberId());
        if (member == null)
        {
            return BadRequest("Can not update member!");
        }

        var uploadResult = await _photoService.AddPhotoAsync(file);
        if (uploadResult.Error != null)
        {
            return BadRequest(uploadResult.Error.Message);
        }

        Photo photo = new Photo
        {
            Url = uploadResult.SecureUrl.AbsoluteUri,
            PublicId = uploadResult.PublicId,
            MemberId = User.GetMemberId()
        };

        member.Photos.Add(photo);

        if (await _uow.Complete())
        {
            return photo.ToPhotoDto();
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        Member? member = await _uow.MembersRepository.GetMemberForUpdateAsync(User.GetMemberId());
        if (member == null)
        {
            return BadRequest("Can not get member from token!");
        }

        Photo? photo = member.Photos.SingleOrDefault(p => p.Id == photoId);
        if (photo == null || photo.Url == member.ImageUrl)
        {
            return BadRequest("Can not set this as main photo!");
        }

        member.ImageUrl = photo.Url;
        member.User.ImageUrl = photo.Url;

        if (await _uow.Complete())
        {
            return NoContent();
        }

        return BadRequest("Problem setting main photo!");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeleteMemberPhoto(int photoId)
    {
        Member? member = await _uow.MembersRepository.GetMemberForUpdateAsync(User.GetMemberId());
        if (member == null) return BadRequest("Can not get member from token!");

        Photo? photo = member.Photos.SingleOrDefault(p => p.Id == photoId);
        if (photo == null) return BadRequest("The member does not own the photo!");
        if (photo.Url == member.ImageUrl) return BadRequest("Can not delete the main photo!");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        member.Photos.Remove(photo);
        if (await _uow.Complete())
        {
            return NoContent();
        }

        return BadRequest("Problem deleting the photo!");
    }
}
