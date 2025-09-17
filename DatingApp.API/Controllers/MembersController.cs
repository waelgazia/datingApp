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

    public MembersController(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
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
}
