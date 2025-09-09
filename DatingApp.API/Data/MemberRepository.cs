using Microsoft.EntityFrameworkCore;

using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Data;

public class MemberRepository (AppDbContext _dbContext) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await _dbContext.Members.FindAsync(id);
    }

    public async Task<IReadOnlyList<Member>> GetMembersAsync()
    {
        return await _dbContext.Members.ToListAsync();
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await _dbContext.Members
            .Where(m => m.Id == memberId)
            .SelectMany(m => m.Photos)
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        _dbContext.Entry(member).State = EntityState.Modified;
    }
}
