using DatingApp.API.Base;
using DatingApp.API.Entities;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Interfaces;

public interface ILikesRepository
{
    Task<MemberLike?> GetMemberLikeAsync(string sourceMemberId, string targetMemberId);

    /// <summary>
    /// Return a list of members that likes the member, or liked by the member.
    /// </summary>
    Task<PagedList<Member>> GetMemberLikesAsync(LikesParameters likesParameters);

    /// <summary>
    /// Get the list of ids that the current member has liked.
    /// </summary>
    Task<IReadOnlyList<string>> GetCurrentMemberLikeIdsAsync(string memberId);

    void AddLike(MemberLike like);
    void DeleteLike(MemberLike like);
    Task<bool> SaveAllChangesAsync();
}
