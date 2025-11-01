using Microsoft.EntityFrameworkCore;

using DatingApp.API.Base;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Data;

public class MemberRepository (AppDbContext _dbContext) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await _dbContext.Members.FindAsync(id);
    }

    public async Task<Member?> GetMemberForUpdateAsync(string id)
    {
        return await _dbContext.Members
            .Include(m => m.User)
            .Include(m => m.Photos)
            .SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<PagedList<Member>>
        GetMembersAsync(MembersResourceParameters membersResourceParameters)
    {
        IQueryable<Member> collection = _dbContext.Members.AsQueryable();
        return await PagedList<Member>.CreateAsync(
            collection,
            membersResourceParameters.PageNumber,
            membersResourceParameters.PageSize);
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
        return await _dbContext.SaveChangesAsync() >= 0;
    }

    public void Update(Member member)
    {
        _dbContext.Entry(member).State = EntityState.Modified;
    }
}
