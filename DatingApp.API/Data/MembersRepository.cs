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
        GetMembersAsync(MembersResourceParameters resourceParameters)
    {
        IQueryable<Member> membersQuery = _dbContext.Members.AsQueryable();
        membersQuery = membersQuery.Where(m => m.Id != resourceParameters.CurrentMemberId);

        DateOnly minDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-resourceParameters.MaxAge - 1));
        DateOnly maxDateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-resourceParameters.MinAge));
        membersQuery = membersQuery.Where(m => m.DateOfBirth >= minDateOfBirth && m.DateOfBirth <= maxDateOfBirth);

        if (!string.IsNullOrWhiteSpace(resourceParameters.Gender))
        {
            membersQuery = membersQuery.Where(m => m.Gender.ToLower() == resourceParameters.Gender.ToLower());
        }

        membersQuery = resourceParameters.OrderBy switch
        {
            "created" => membersQuery.OrderByDescending(m => m.Created),
            _ => membersQuery.OrderByDescending(m => m.LastActive)
        };

        return await PagedList<Member>.CreateAsync(
            membersQuery,
            resourceParameters.PageNumber,
            resourceParameters.PageSize);
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
