using DatingApp.API.Entities;

namespace DatingApp.API.Interfaces;

public interface IMemberRepository
{
    Task<IReadOnlyList<Member>> GetMembersAsync();
    Task<Member?> GetMemberByIdAsync(string id);
    Task<Member?> GetMemberForUpdateAsync(string id);
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
    void Update(Member member);
    Task<bool> SaveAllAsync();
}
