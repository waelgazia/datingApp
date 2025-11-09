using DatingApp.API.Base;
using DatingApp.API.Entities;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Interfaces;

public interface IMembersRepository
{
    Task<PagedList<Member>> GetMembersAsync(MembersResourceParameters membersResourceParameters);
    Task<Member?> GetMemberByIdAsync(string id);
    Task<Member?> GetMemberForUpdateAsync(string id);
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
    void Update(Member member);
    Task<bool> SaveAllAsync();
}
