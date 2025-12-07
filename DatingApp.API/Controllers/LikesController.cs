using Microsoft.AspNetCore.Mvc;

using DatingApp.API.DTOs;
using DatingApp.API.Base;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Controllers;

public class LikesController(IUnitOfWork _uow) : BaseApiController
{
    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberIds()
    {
        return Ok(await _uow.LikesRepository.GetCurrentMemberLikeIdsAsync(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>>
        GetMemberLikes([FromQuery] LikesParameters likesParameters)
    {
        likesParameters.MemberId = User.GetMemberId();
        PagedList<Member> paginatedMembers = await _uow.LikesRepository.GetMemberLikesAsync(likesParameters);

        AddPaginationHeader(paginatedMembers);
        return Ok(paginatedMembers.ToMembersDto());
    }

    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        string sourceMemberId = User.GetMemberId();
        if (sourceMemberId == targetMemberId)
            return BadRequest("You cannot like yourself");

        MemberLike? existingLike = await _uow.LikesRepository.GetMemberLikeAsync(sourceMemberId, targetMemberId);
        if (existingLike == null)
        {
            MemberLike newLike = new MemberLike
            {
                SourceMemberId = sourceMemberId,
                TargetMemberId = targetMemberId
            };

            _uow.LikesRepository.AddLike(newLike);
        }
        else
        {
            _uow.LikesRepository.DeleteLike(existingLike);
        }

        if (!await _uow.Complete())
        {
            return BadRequest("Failed to update like");
        }

        return Ok();
    }
}
