using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using DatingApp.API.DTOs;
using DatingApp.API.Base;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.Extensions;

namespace DatingApp.API.Controllers;

[Authorize]
public class MembersController : BaseApiController
{
    private readonly IMemberRepository _memberRepository;
    private readonly IPhotoService _photoService;

    public MembersController(IMemberRepository memberRepository, IPhotoService photoService)
    {
        _memberRepository = memberRepository;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetMembers()
    {
        IReadOnlyList<Member> memberEntities = await _memberRepository.GetMembersAsync();
        return Ok(memberEntities.ToMembersDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetMember(string id)
    {
        Member? member = await _memberRepository.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }

        return Ok(member.ToMemberDto());
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetMemberPhotos(string id)
    {
        IReadOnlyList<Photo> photoEntities = await _memberRepository.GetPhotosForMemberAsync(id);
        return Ok(photoEntities.ToPhotosDto());
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberForUpdateDto memberUpdateDto)
    {
        string memberId = User.GetMemberId();
        Member? memberEntity = await _memberRepository.GetMemberForUpdateAsync(memberId);

        if (memberEntity == null)
        {
            return BadRequest("Could not get member");
        }

        memberEntity.DisplayName = memberUpdateDto.DisplayName ?? memberEntity.DisplayName;
        memberEntity.Description = memberUpdateDto.Description ?? memberEntity.Description;
        memberEntity.City = memberUpdateDto.City ?? memberEntity.City;
        memberEntity.Country = memberUpdateDto.Country ?? memberEntity.Country;

        memberEntity.User.DisplayName = memberUpdateDto.DisplayName ?? memberEntity.User.DisplayName;

        if (!await _memberRepository.SaveAllAsync())
        {
            return BadRequest("Failed to update member!");
        }

        return NoContent();
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] IFormFile file)
    {
        Member? member = await _memberRepository.GetMemberForUpdateAsync(User.GetMemberId());
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

        if (member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }

        member.Photos.Add(photo);

        if (await _memberRepository.SaveAllAsync())
        {
            return photo.ToPhotoDto();
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        Member? member = await _memberRepository.GetMemberForUpdateAsync(User.GetMemberId());
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

        if (await _memberRepository.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem setting main photo!");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeleteMemberPhoto(int photoId)
    {
        Member? member = await _memberRepository.GetMemberForUpdateAsync(User.GetMemberId());
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
        if (await _memberRepository.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem deleting the photo!");
    }
}
