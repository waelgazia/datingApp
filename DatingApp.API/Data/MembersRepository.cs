using Microsoft.EntityFrameworkCore;

using DatingApp.API.Base;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.ResourceParameters;

namespace DatingApp.API.Data;

public class MembersRepository(AppDbContext _dbContext) : IMembersRepository
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
        GetMembersAsync(MembersParameters membersParameters)
    {
        IQueryable<Member> membersQuery = _dbContext.Members.AsQueryable();
        membersQuery = membersQuery.Where(m => m.Id != membersParameters.CurrentMemberId);

        DateOnly minDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-membersParameters.MaxAge - 1));
        DateOnly maxDateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-membersParameters.MinAge));
        membersQuery = membersQuery.Where(m => m.DateOfBirth >= minDateOfBirth && m.DateOfBirth <= maxDateOfBirth);

        if (!string.IsNullOrWhiteSpace(membersParameters.Gender))
        {
            membersQuery = membersQuery.Where(m => m.Gender.ToLower() == membersParameters.Gender.ToLower());
        }

        membersQuery = membersParameters.OrderBy switch
        {
            "created" => membersQuery.OrderByDescending(m => m.Created),
            _ => membersQuery.OrderByDescending(m => m.LastActive)
        };

        return await PagedList<Member>.CreateAsync(
            membersQuery,
            membersParameters.PageNumber,
            membersParameters.PageSize);
    }

    public async Task<IReadOnlyList<Photo>>
        GetPhotosForMemberAsync(string memberId, bool forLoggedInUser = false)
    {
        IQueryable<Photo> memberPhotosQuery = _dbContext.Members
                .Where(m => m.Id == memberId)
                .SelectMany(m => m.Photos);

        if (forLoggedInUser) memberPhotosQuery = memberPhotosQuery.IgnoreQueryFilters();

        return await memberPhotosQuery.ToListAsync();
    }

    public void Update(Member member)
    {
        _dbContext.Entry(member).State = EntityState.Modified;
    }
}
