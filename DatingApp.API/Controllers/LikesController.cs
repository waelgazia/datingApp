using Microsoft.AspNetCore.Mvc;

using DatingApp.API.DTOs;
using DatingApp.API.Base;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Controllers;

public class LikesController : BaseApiController
{
    private readonly ILikesRepository _likesRepository;

    public LikesController(ILikesRepository likesRepository)
    {
        _likesRepository = likesRepository;
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberIds()
    {
        return Ok(await _likesRepository.GetCurrentMemberLikeIdsAsync(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>>
        GetMemberLikes([FromQuery] LikesResourceParameters likesResourceParameters)
    {
        likesResourceParameters.MemberId = User.GetMemberId();
        PagedList<Member> paginatedMembers = await _likesRepository
            .GetMemberLikesAsync(likesResourceParameters);

        AddPaginationHeader(paginatedMembers);
        return Ok(paginatedMembers.ToMembersDto());
    }

    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        string sourceMemberId = User.GetMemberId();
        if (sourceMemberId == targetMemberId)
            return BadRequest("You cannot like yourself");

        MemberLike? existingLike = await _likesRepository.GetMemberLikeAsync(sourceMemberId, targetMemberId);
        if (existingLike == null)
        {
            MemberLike newLike = new MemberLike
            {
                SourceMemberId = sourceMemberId,
                TargetMemberId = targetMemberId
            };

            _likesRepository.AddLike(newLike);
        }
        else
        {
            _likesRepository.DeleteLike(existingLike);
        }

        if (!await _likesRepository.SaveAllChangesAsync())
        {
            return BadRequest("Failed to update like");
        }

        return Ok();
    }
}
