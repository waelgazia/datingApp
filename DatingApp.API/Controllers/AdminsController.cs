using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using DatingApp.API.Base;
using DatingApp.API.Globals;
using DatingApp.API.Entities;

namespace DatingApp.API.Controllers;

public class AdminsController(UserManager<AppUser> _userManager) : BaseApiController
{
    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("admin-secret")]
    public ActionResult<string> GetAdminSecret()
    {
        return Ok("Only admins can see this!");
    }

    [Authorize(Policy = Policies.REQUIRE_ADMIN_ROLE)]
    [HttpGet("users-with-roles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new
            {
                user.Id,
                user.Email,
                Roles = roles.ToList()
            });
        }

        return Ok(userList);
    }

    [Authorize(Policy = Policies.REQUIRE_ADMIN_ROLE)]
    [HttpPost("edit-roles/{userId}")]
    public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery] string roles)
    {
        if (string.IsNullOrWhiteSpace(roles))
            return BadRequest("You must select at least one role");

        string[] selectedRoles = roles.Split(',').ToArray();

        AppUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("Could not retrieve the user");

        // add the new roles, and delete the old ones.
        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to update user roles!");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove old roles!");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = Policies.MODERATE_PHOTO_ROLE)]
    [HttpGet("photos-to-moderate")]
    public IActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this!");
    }
}


public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}