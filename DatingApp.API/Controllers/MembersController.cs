using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using DatingApp.API.Data;
using DatingApp.API.Base;
using DatingApp.API.Entities;

namespace DatingApp.API.Controllers;

[Authorize]
public class MembersController : BaseApiController
{
    private readonly AppDbContext _dbContext;

    public MembersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        return Ok(await _dbContext.Users.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetMember(string id)
    {
        AppUser? user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
