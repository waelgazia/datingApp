using Microsoft.EntityFrameworkCore;

using DatingApp.API.Base;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Data;

public class LikesRepository(AppDbContext _dbContext) : ILikesRepository
{
    public async Task<MemberLike?> GetMemberLikeAsync(string sourceMemberId, string targetMemberId)
    {
        return await _dbContext.MemberLikes.FindAsync(sourceMemberId, targetMemberId);
    }

    public async Task<PagedList<Member>>
        GetMemberLikesAsync(LikesParameters likesParameters)
    {
        IQueryable<MemberLike> likesQuery = _dbContext.MemberLikes.AsQueryable();
        IQueryable<Member> membersQuery;

        switch (likesParameters.Predicate)
        {
            case LikePredicate.LIKED:
                membersQuery = likesQuery
                    .Where(like => like.SourceMemberId == likesParameters.MemberId)
                    .Select(like => like.TargetMember);
                break;

            case LikePredicate.LIKED_BY:
                membersQuery = likesQuery
                    .Where(like => like.TargetMemberId == likesParameters.MemberId)
                    .Select(like => like.SourceMember).AsQueryable();
                break;

            default: // LikePredicate.MUTUAL (members that like the memberId and the other way around)
                IReadOnlyList<string> likeIds = await GetCurrentMemberLikeIdsAsync(likesParameters.MemberId);

                membersQuery = likesQuery
                    .Where(like => like.TargetMemberId == likesParameters.MemberId
                        && likeIds.Contains(like.SourceMemberId))
                    .Select(like => like.SourceMember);
                break;
        }

        return await PagedList<Member>.CreateAsync(
            membersQuery,
            likesParameters.PageNumber,
            likesParameters.PageSize
        );
    }

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIdsAsync(string memberId)
    {
        return await _dbContext.MemberLikes
            .Where(like => like.SourceMemberId == memberId)
            .Select(like => like.TargetMemberId)
            .ToListAsync();
    }

    public void AddLike(MemberLike like)
    {
        _dbContext.MemberLikes.Add(like);
    }

    public void DeleteLike(MemberLike like)
    {
        _dbContext.MemberLikes.Remove(like);
    }
}
