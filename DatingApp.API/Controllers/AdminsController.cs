using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using DatingApp.API.Base;
using DatingApp.API.DTOs;
using DatingApp.API.Globals;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Controllers;

public class AdminsController(
    UserManager<AppUser> _userManager,
    IUnitOfWork _uow, IPhotoService
    _photoService) : BaseApiController
{
    [Authorize(Policy = Policies.REQUIRE_ADMIN_ROLE)]
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

    [Authorize(Policy = Policies.ADMIN_OR_MODERATION_ROLE)]
    [HttpGet("unapproved-photos")]
    public async Task<ActionResult<IReadOnlyList<PhotoForApprovalDto>>> GetUnapprovedPhotos()
    {
        IReadOnlyList<Photo> photos = await _uow.PhotosRepository.GetUnapprovedPhotosAsync();
        return Ok(photos.ToPhotosForCreationDto());
    }

    [Authorize(Policy = Policies.ADMIN_OR_MODERATION_ROLE)]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<IActionResult> ApprovePhoto(int photoId)
    {
        Photo? photo = await _uow.PhotosRepository.GetPhotoByIdAsync(photoId);
        if (photo == null) return BadRequest("Can not find the photo");
        photo.IsApproved = true;

        Member? member = await _uow.MembersRepository.GetMemberForUpdateAsync(photo.MemberId);
        if (member == null) return BadRequest("The member does not own the photo");

        // update the member main photo if he does not have one.
        if (member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }

        if (!await _uow.Complete())
        {
            return BadRequest("Failed to approve photo");
        }

        return Ok();
    }

    [Authorize(Policy = Policies.ADMIN_OR_MODERATION_ROLE)]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<IActionResult> RejectPhoto(int photoId)
    {
        Photo? photo = await _uow.PhotosRepository.GetPhotoByIdAsync(photoId);
        if (photo == null) return BadRequest("Can not find the photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Result == "ok")
            {
                _uow.PhotosRepository.DeletePhoto(photo);
            }
        }
        else
        {
            _uow.PhotosRepository.DeletePhoto(photo);
        }

        if (!await _uow.Complete())
        {
            return BadRequest("Problem deleting the photo");
        }

        return NoContent();
    }
}